using gec.Application.Contracts.Infrastructure.Canvas.OAuth;
using gec.Application.Contracts.Infrastructure.Canvas.OAuth.Models;
using gec.Application.Contracts.Server.Session;
using gec.Server.Integrations.Canvas;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace gec.Test.Server.Integrations
{
    [TestFixture]
    public class CanvasControllerTests
    {
        private Mock<ICanvasOAuthService> _mockCanvasOAuthService;
        private Mock<ISessionStorageService> _mockSessionStorageService;
        private CanvasController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockCanvasOAuthService = new Mock<ICanvasOAuthService>();
            _mockSessionStorageService = new Mock<ISessionStorageService>();
            _controller = new CanvasController(_mockCanvasOAuthService.Object, _mockSessionStorageService.Object);
        }

        [Test]
        public void AuthorizeUser_ShouldRedirectToAuthorizationUrl_WhenCalled()
        {
            // Arrange
            var expectedUrl = "https://example.com/authorize";
            _mockCanvasOAuthService.Setup(s => s.BuildAuthorizationUrl()).Returns(expectedUrl);

            // Act
            var result = _controller.AuthorizeUser();

            // Assert
            Assert.IsInstanceOf<RedirectResult>(result);
            var redirectResult = result as RedirectResult;
            Assert.AreEqual(expectedUrl, redirectResult.Url);
            _mockCanvasOAuthService.Verify(s => s.BuildAuthorizationUrl(), Times.Once);
        }

        [Test]
        public async Task HandleOAuthCallback_ShouldReturnError_WhenTokenResponseFails()
        {
            // Arrange
            var queryParameters = new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { "code", "exampleCode" }
            };
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Query = new QueryCollection(queryParameters);
            _controller.ControllerContext.HttpContext = httpContext;

            var failedTokenResponse =
                CSharpFunctionalExtensions.Result.Failure<CanvasAuthToken>("Error retrieving token");
            _mockCanvasOAuthService.Setup(s => s.HandleOAuthCallbackAsync(It.IsAny<Dictionary<string, string>>()))
                .ReturnsAsync(failedTokenResponse);

            // Act
            var result = await _controller.HandleOAuthCallback();

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var errorResult = result as ObjectResult;
            Assert.AreEqual(400, errorResult.StatusCode);
            Assert.AreEqual("Error retrieving token", ((gec.Server.Common.Envelope)errorResult.Value).ErrorMessage);
            _mockCanvasOAuthService.Verify(s => s.HandleOAuthCallbackAsync(It.IsAny<Dictionary<string, string>>()),
                Times.Once);
        }

        [Test]
        public async Task HandleOAuthCallback_ShouldRedirectToHome_WhenTokenResponseSucceeds()
        {
            // Arrange
            var queryParameters = new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>
            {
                { "code", "exampleCode" }
            };
            var httpContext = new DefaultHttpContext();
            httpContext.Request.Query = new QueryCollection(queryParameters);
            _controller.ControllerContext.HttpContext = httpContext;

            var tokenResponse =
                CSharpFunctionalExtensions.Result.Success(new CanvasAuthToken { AccessToken = "testToken" });
            _mockCanvasOAuthService.Setup(s => s.HandleOAuthCallbackAsync(It.IsAny<Dictionary<string, string>>()))
                .ReturnsAsync(tokenResponse);

            // Act
            var result = await _controller.HandleOAuthCallback();

            // Assert
            Assert.IsInstanceOf<RedirectResult>(result);
            var redirectResult = result as RedirectResult;
            Assert.AreEqual("/", redirectResult.Url);
            _mockSessionStorageService.Verify(s => s.Store("CanvasAuthToken", tokenResponse.Value), Times.Once);
        }

        [Test]
        public async Task ValidateOrRefreshToken_ShouldRedirectToAuthorizationUrl_WhenTokenIsNotStored()
        {
            // Arrange
            _mockSessionStorageService.Setup(s => s.Retrieve<CanvasAuthToken>("CanvasAuthToken"))
                .Returns(CSharpFunctionalExtensions.Result.Failure<CanvasAuthToken>("Token not found"));

            var authorizationUrl = "https://example.com/authorize";
            _mockCanvasOAuthService.Setup(s => s.BuildAuthorizationUrl()).Returns(authorizationUrl);

            // Act
            var result = await _controller.ValidateOrRefreshToken();

            // Assert
            Assert.IsInstanceOf<RedirectResult>(result);
            var redirectResult = result as RedirectResult;
            Assert.AreEqual(authorizationUrl, redirectResult.Url);
            _mockCanvasOAuthService.Verify(s => s.BuildAuthorizationUrl(), Times.Once);
            _mockSessionStorageService.Verify(s => s.Retrieve<CanvasAuthToken>("CanvasAuthToken"), Times.Once);
        }

        [Test]
        public async Task ValidateOrRefreshToken_ShouldRedirectToAuthorizationUrl_WhenTokenRefreshFails()
        {
            // Arrange
            var existingToken = new CanvasAuthToken { AccessToken = "expiredToken" };
            _mockSessionStorageService.Setup(s => s.Retrieve<CanvasAuthToken>("CanvasAuthToken"))
                .Returns(CSharpFunctionalExtensions.Result.Success(existingToken));

            var failedTokenRefresh = CSharpFunctionalExtensions.Result.Failure<CanvasAuthToken>("Token refresh failed");
            _mockCanvasOAuthService.Setup(s => s.GetTokenAsync(existingToken)).ReturnsAsync(failedTokenRefresh);

            var authorizationUrl = "https://example.com/authorize";
            _mockCanvasOAuthService.Setup(s => s.BuildAuthorizationUrl()).Returns(authorizationUrl);

            // Act
            var result = await _controller.ValidateOrRefreshToken();

            // Assert
            Assert.IsInstanceOf<RedirectResult>(result);
            var redirectResult = result as RedirectResult;
            Assert.AreEqual(authorizationUrl, redirectResult.Url);
            _mockSessionStorageService.Verify(s => s.Retrieve<CanvasAuthToken>("CanvasAuthToken"), Times.Once);
            _mockCanvasOAuthService.Verify(s => s.GetTokenAsync(existingToken), Times.Once);
            _mockCanvasOAuthService.Verify(s => s.BuildAuthorizationUrl(), Times.Once);
        }

        [Test]
        public async Task ValidateOrRefreshToken_ShouldRedirectToHome_WhenTokenRefreshSucceeds()
        {
            // Arrange
            var existingToken = new CanvasAuthToken { AccessToken = "expiredToken" };
            var refreshedToken = new CanvasAuthToken { AccessToken = "refreshedToken" };
            _mockSessionStorageService.Setup(s => s.Retrieve<CanvasAuthToken>("CanvasAuthToken"))
                .Returns(CSharpFunctionalExtensions.Result.Success(existingToken));

            _mockCanvasOAuthService.Setup(s => s.GetTokenAsync(existingToken))
                .ReturnsAsync(CSharpFunctionalExtensions.Result.Success(refreshedToken));

            // Act
            var result = await _controller.ValidateOrRefreshToken();

            // Assert
            Assert.IsInstanceOf<RedirectResult>(result);
            var redirectResult = result as RedirectResult;
            Assert.AreEqual("/", redirectResult.Url);
            _mockSessionStorageService.Verify(s => s.Retrieve<CanvasAuthToken>("CanvasAuthToken"), Times.Once);
            _mockCanvasOAuthService.Verify(s => s.GetTokenAsync(existingToken), Times.Once);
            _mockSessionStorageService.Verify(s => s.Store("CanvasAuthToken", refreshedToken), Times.Once);
        }
    }
}