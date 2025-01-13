using CSharpFunctionalExtensions;
using gec.Application.Contracts.Infrastructure.Federation;
using gec.Application.Contracts.Infrastructure.Federation.Models;
using gec.Server.Common;
using gec.Server.Integrations.Federation;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace gec.Test.Server.Integrations;

[TestFixture]
public class FederationAuthControllerTests
{
    [SetUp]
    public void SetUp()
    {
        _mockFederationService = new Mock<IFederationService>();
        _controller = new FederationAuthController(_mockFederationService.Object);
    }

    private Mock<IFederationService> _mockFederationService;
    private FederationAuthController _controller;

    [Test]
    public async Task Get_ShouldReturnError_WhenFederationServiceFails()
    {
        // Arrange
        var errorMessage = "Federation service error";
        _mockFederationService
            .Setup(s => s.HandleAuthAsync())
            .ReturnsAsync(Result.Failure<FederationContext>(errorMessage));

        // Act
        var result = await _controller.Get();

        // Assert
        Assert.IsInstanceOf<ObjectResult>(result);
        var objectResult = result as ObjectResult;
        Assert.AreEqual(400, objectResult.StatusCode);
        Assert.AreEqual(errorMessage, ((Envelope)objectResult.Value).ErrorMessage);
    }

    [Test]
    public async Task Get_ShouldReturnOk_WhenFederationServiceSucceeds()
    {
        // Arrange
        var federationContext = new FederationContext { User = new User { Name = "TestUser", UserId = "TestUserId" } };
        _mockFederationService
            .Setup(s => s.HandleAuthAsync())
            .ReturnsAsync(Result.Success<FederationContext>(federationContext));

        // Act
        var result = await _controller.Get();

        // Assert
        Assert.IsInstanceOf<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.AreSame(federationContext, ((Envelope<FederationContext>)okResult.Value).Result);
    }
}