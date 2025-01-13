using CSharpFunctionalExtensions;
using FluentAssertions;
using gec.Application.Contracts.Infrastructure.Federation;
using gec.Application.Contracts.Infrastructure.Federation.Models;
using gec.Server.Common;
using gec.Server.Integrations.Federation;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace gec.Test.Server.Integrations.Federation;

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
        const string errorMessage = TestConstants.FEDERATION_SERVICE_ERROR;
        _mockFederationService
            .Setup(s => s.HandleAuthAsync())
            .ReturnsAsync(Result.Failure<FederationContext>(errorMessage));

        // Act
        var result = await _controller.Get();

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.StatusCode.Should().Be(400);
        ((Envelope)badRequestResult.Value).ErrorMessage.Should().Be(errorMessage);

        // Verify that the service method was called once
        _mockFederationService.Verify(s => s.HandleAuthAsync(), Times.Once);
    }

    [Test]
    public async Task Get_ShouldReturnOk_WhenFederationServiceSucceeds()
    {
        // Arrange
        var federationContext = TestHelper.GetSampleFederationContext();
        _mockFederationService
            .Setup(s => s.HandleAuthAsync())
            .ReturnsAsync(Result.Success(federationContext));

        // Act
        var result = await _controller.Get();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        ((Envelope<FederationContext>)okResult.Value).Result.Should().BeEquivalentTo(federationContext);

        // Verify that the service method was called once
        _mockFederationService.Verify(s => s.HandleAuthAsync(), Times.Once);
    }
}

public static class TestConstants
{
    public const string FEDERATION_SERVICE_ERROR = "Federation service error";
}

public static class TestHelper
{
    public static FederationContext GetSampleFederationContext() =>
        new()
        {
            User = new User
            {
                Name = "TestUser",
                UserId = "TestUserId"
            }
        };
}
