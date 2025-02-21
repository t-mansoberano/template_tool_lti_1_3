using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using gec.Server.Integrations.Lti;
using gec.Application.Contracts.Infrastructure.Lti;
using gec.Application.Contracts.Infrastructure.Lti.Models;
using gec.Application.Contracts.Server.Configuration;
using gec.Application.Contracts.Server.Configuration.Models;
using gec.Application.Contracts.Server.Fake;
using gec.Application.Contracts.Server.Session;

namespace gec.Test.Server.Integrations.Lti;

    [TestFixture]
    public class LtiControllerTests
    {
        private Mock<ILtiService> _mockLtiService;
        private Mock<ISessionStorageService> _mockSessionStorageService;
        private Mock<IAppSettingsService> _mockAppSettingsService;
        private Mock<IFakeDataService> _mockFakeDataService;
        private LtiController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockLtiService = new Mock<ILtiService>();
            _mockSessionStorageService = new Mock<ISessionStorageService>();
            _mockAppSettingsService = new Mock<IAppSettingsService>();
            _mockFakeDataService = new Mock<IFakeDataService>();
            
            _controller = new LtiController(
                _mockLtiService.Object,
                _mockSessionStorageService.Object,
                _mockAppSettingsService.Object,
                _mockFakeDataService.Object
            );
        }

        [Test]
        public void Get_ShouldReturnFakeData_WhenFakeLtiIsEnabled()
        {
            // Arrange
            var fakeSettings = new FakeSettings { UseFakeLti = true }; // ✅ Instancia real
            _mockAppSettingsService.Setup(a => a.Fake).Returns(fakeSettings); // ✅ Retornar instancia real

            var fakeLtiContext = new LtiContext { User = new User { Name = "Fake User" } };
            _mockFakeDataService.Setup(f => f.GetFakeData<LtiContext>(It.IsAny<string>()))
                .Returns(CSharpFunctionalExtensions.Result.Success(fakeLtiContext));
            
            // Act
            var result = _controller.Get() as OkObjectResult;
            
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual(fakeLtiContext, ((gec.Server.Common.Envelope<LtiContext>) result.Value).Result);
        }

        [Test]
        public void Get_ShouldReturnError_WhenLtiContextIsMissing()
        {
            // Arrange
            var fakeSettings = new FakeSettings { UseFakeLti = false }; // Usar un objeto real
            _mockAppSettingsService.Setup(a => a.Fake).Returns(fakeSettings); // Retornar el objeto real

            _mockSessionStorageService.Setup(s => s.Retrieve<LtiContext>(It.IsAny<string>()))
                .Returns(CSharpFunctionalExtensions.Result.Failure<LtiContext>("LtiContext not found"));

            // Act
            var result = _controller.Get() as ObjectResult;
            
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
        }

        [Test]
        public void Get_ShouldReturnLtiContext_WhenStored()
        {
            // Simula que FakeSettings no es nulo
            var fakeSettings = new FakeSettings { UseFakeLti = false };
            _mockAppSettingsService.Setup(a => a.Fake).Returns(fakeSettings);
            
            // Arrange
            var storedLtiContext = new LtiContext { User = new User { Name = "Stored User" } };
            _mockSessionStorageService.Setup(s => s.Retrieve<LtiContext>(It.IsAny<string>()))
                .Returns(CSharpFunctionalExtensions.Result.Success(storedLtiContext));
            
            // Act
            var result = _controller.Get() as OkObjectResult;
            
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(200, result.StatusCode);
            Assert.AreEqual(storedLtiContext, ((gec.Server.Common.Envelope<LtiContext>) result.Value).Result);
        }

        [Test]
        public void LaunchLTI_ShouldReturnError_WhenAuthorizationUrlFails()
        {
            // Arrange
            var form = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>());
            var formModel = new LoginInitiationResponse(new Dictionary<string, string>());
            _mockLtiService.Setup(l => l.BuildAuthorizationUrl(It.IsAny<LoginInitiationResponse>()))
                .Returns(CSharpFunctionalExtensions.Result.Failure<string>("Invalid request"));
            
            // Act
            var result = _controller.LaunchLTI(form) as ObjectResult;
            
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
        }

        [Test]
        public void LaunchLTI_ShouldRedirect_WhenAuthorizationUrlIsValid()
        {
            // Arrange
            var form = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>());
            var redirectUrl = "https://example.com/auth";
            _mockLtiService.Setup(l => l.BuildAuthorizationUrl(It.IsAny<LoginInitiationResponse>()))
                .Returns(CSharpFunctionalExtensions.Result.Success(redirectUrl));
            
            // Act
            var result = _controller.LaunchLTI(form) as RedirectResult;
            
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(redirectUrl, result.Url);
        }

        [Test]
        public async Task HandleRedirect_ShouldReturnError_WhenTokenValidationFails()
        {
            // Arrange
            var form = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>());
            _mockLtiService.Setup(l => l.HandleRedirectAsync(It.IsAny<Dictionary<string, string>>()))
                .ReturnsAsync(CSharpFunctionalExtensions.Result.Failure<LtiContext>("Invalid token"));
            
            // Act
            var result = await _controller.HandleRedirect(form) as ObjectResult;
            
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual(400, result.StatusCode);
        }

        [Test]
        public async Task HandleRedirect_ShouldStoreContextAndRedirect_WhenSuccessful()
        {
            // Arrange
            var form = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>());
            var ltiContext = new LtiContext { User = new User { Name = "Valid User" } };
            _mockLtiService.Setup(l => l.HandleRedirectAsync(It.IsAny<Dictionary<string, string>>()))
                .ReturnsAsync(CSharpFunctionalExtensions.Result.Success(ltiContext));
            
            // Act
            var result = await _controller.HandleRedirect(form) as RedirectResult;
            
            // Assert
            Assert.IsNotNull(result);
            Assert.AreEqual("/api/lti/oauth/token/validate", result.Url);
            _mockSessionStorageService.Verify(s => s.Store(LtiContext.Key, ltiContext), Times.Once);
        }
    }