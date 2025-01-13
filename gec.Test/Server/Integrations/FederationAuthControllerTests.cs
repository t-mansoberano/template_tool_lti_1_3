using gec.Application.Contracts.Infrastructure.Federation;
using gec.Application.Contracts.Infrastructure.Federation.Models;
using gec.Server.Integrations.Federation;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace gec.Test.Server.Integrations
{
    [TestFixture]
    public class FederationAuthControllerTests
    {
        private Mock<IFederationService> _mockFederationService;
        private FederationAuthController _controller;

        [SetUp]
        public void SetUp()
        {
            _mockFederationService = new Mock<IFederationService>();
            _controller = new FederationAuthController(_mockFederationService.Object);
        }

        [Test]
        public async Task Get_ShouldReturnError_WhenFederationServiceFails()
        {
            // Arrange
            var errorMessage = "Federation service error";
            _mockFederationService
                .Setup(s => s.HandleAuthAsync())
                .ReturnsAsync(CSharpFunctionalExtensions.Result.Failure<FederationContext>(errorMessage));

            // Act
            var result = await _controller.Get();

            // Assert
            Assert.IsInstanceOf<ObjectResult>(result);
            var objectResult = result as ObjectResult;
            Assert.AreEqual(400, objectResult.StatusCode);
            Assert.AreEqual(errorMessage, ((gec.Server.Common.Envelope)objectResult.Value).ErrorMessage);
        }

        [Test]
        public async Task Get_ShouldReturnOk_WhenFederationServiceSucceeds()
        {
            // Arrange
            var federationContext = new FederationContext () { User = new User() { Name = "TestUser", UserId = "TestUserId"} };
            _mockFederationService
                .Setup(s => s.HandleAuthAsync())
                .ReturnsAsync(CSharpFunctionalExtensions.Result.Success<FederationContext>(federationContext));

            // Act
            var result = await _controller.Get();

            // Assert
            Assert.IsInstanceOf<OkObjectResult>(result);
            var okResult = result as OkObjectResult;
            Assert.AreSame(federationContext, ((gec.Server.Common.Envelope<FederationContext>)okResult.Value).Result);
        }
    }
}
