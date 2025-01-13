using CSharpFunctionalExtensions;
using FluentAssertions;
using gec.Application.Contracts.Infrastructure.Canvas.OAuth;
using gec.Application.Contracts.Infrastructure.Canvas.OAuth.Models;
using gec.Application.Contracts.Server.Session;
using gec.Server.Common;
using gec.Server.Integrations.Canvas;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Moq;

namespace gec.Test.Server.Integrations.Canvas;

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
        const string expectedUrl = TestConstants.AUTHORIZATION_URL;
        _mockCanvasOAuthService.Setup(s => s.BuildAuthorizationUrl()).Returns(expectedUrl);

        // Act
        var result = _controller.AuthorizeUser();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Url.Should().Be(expectedUrl);
        _mockCanvasOAuthService.Verify(s => s.BuildAuthorizationUrl(), Times.Once);
    }

    [Test]
    public async Task HandleOAuthCallback_ShouldReturnError_WhenTokenResponseFails()
    {
        // Arrange
        var queryParameters = TestHelper.GetQueryParameters("exampleCode");
        _controller.ControllerContext.HttpContext = TestHelper.GetHttpContext(queryParameters);

        const string errorMessage = TestConstants.TOKEN_ERROR_MESSAGE;
        var failedTokenResponse = Result.Failure<CanvasAuthToken>(errorMessage);
        _mockCanvasOAuthService.Setup(s => s.HandleOAuthCallbackAsync(It.IsAny<Dictionary<string, string>>()))
            .ReturnsAsync(failedTokenResponse);

        // Act
        var result = await _controller.HandleOAuthCallback();

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var errorResult = result as ObjectResult;
        errorResult.StatusCode.Should().Be(400);
        ((Envelope)errorResult.Value).ErrorMessage.Should().Be(errorMessage);
        _mockCanvasOAuthService.Verify(s => s.HandleOAuthCallbackAsync(It.IsAny<Dictionary<string, string>>()), Times.Once);
    }

    [Test]
    public async Task HandleOAuthCallback_ShouldRedirectToHome_WhenTokenResponseSucceeds()
    {
        // Arrange
        var queryParameters = TestHelper.GetQueryParameters("exampleCode");
        _controller.ControllerContext.HttpContext = TestHelper.GetHttpContext(queryParameters);

        var tokenResponse = Result.Success(new CanvasAuthToken { AccessToken = TestConstants.TEST_TOKEN });
        _mockCanvasOAuthService.Setup(s => s.HandleOAuthCallbackAsync(It.IsAny<Dictionary<string, string>>()))
            .ReturnsAsync(tokenResponse);

        // Act
        var result = await _controller.HandleOAuthCallback();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Url.Should().Be("/");
        _mockSessionStorageService.Verify(s => s.Store("CanvasAuthToken", tokenResponse.Value), Times.Once);
    }

    [Test]
    public async Task ValidateOrRefreshToken_ShouldRedirectToAuthorizationUrl_WhenTokenIsNotStored()
    {
        // Arrange
        _mockSessionStorageService.Setup(s => s.Retrieve<CanvasAuthToken>("CanvasAuthToken"))
            .Returns(Result.Failure<CanvasAuthToken>(TestConstants.TOKEN_NOT_FOUND_MESSAGE));

        const string authorizationUrl = TestConstants.AUTHORIZATION_URL;
        _mockCanvasOAuthService.Setup(s => s.BuildAuthorizationUrl()).Returns(authorizationUrl);

        // Act
        var result = await _controller.ValidateOrRefreshToken();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Url.Should().Be(authorizationUrl);
        _mockCanvasOAuthService.Verify(s => s.BuildAuthorizationUrl(), Times.Once);
        _mockSessionStorageService.Verify(s => s.Retrieve<CanvasAuthToken>("CanvasAuthToken"), Times.Once);
    }

    [Test]
    public async Task ValidateOrRefreshToken_ShouldRedirectToAuthorizationUrl_WhenTokenRefreshFails()
    {
        // Arrange
        var existingToken = new CanvasAuthToken { AccessToken = "expiredToken" };
        _mockSessionStorageService.Setup(s => s.Retrieve<CanvasAuthToken>("CanvasAuthToken"))
            .Returns(Result.Success(existingToken));

        const string failedMessage = TestConstants.TOKEN_REFRESH_FAILED_MESSAGE;
        var failedTokenRefresh = Result.Failure<CanvasAuthToken>(failedMessage);
        _mockCanvasOAuthService.Setup(s => s.GetTokenAsync(existingToken)).ReturnsAsync(failedTokenRefresh);

        const string authorizationUrl = TestConstants.AUTHORIZATION_URL;
        _mockCanvasOAuthService.Setup(s => s.BuildAuthorizationUrl()).Returns(authorizationUrl);

        // Act
        var result = await _controller.ValidateOrRefreshToken();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Url.Should().Be(authorizationUrl);
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
            .Returns(Result.Success(existingToken));

        _mockCanvasOAuthService.Setup(s => s.GetTokenAsync(existingToken))
            .ReturnsAsync(Result.Success(refreshedToken));

        // Act
        var result = await _controller.ValidateOrRefreshToken();

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Url.Should().Be("/");
        _mockSessionStorageService.Verify(s => s.Retrieve<CanvasAuthToken>("CanvasAuthToken"), Times.Once);
        _mockCanvasOAuthService.Verify(s => s.GetTokenAsync(existingToken), Times.Once);
        _mockSessionStorageService.Verify(s => s.Store("CanvasAuthToken", refreshedToken), Times.Once);
    }
}

public static class TestConstants
{
    public const string AUTHORIZATION_URL = "https://example.com/authorize";
    public const string TOKEN_ERROR_MESSAGE = "Error retrieving token";
    public const string TOKEN_NOT_FOUND_MESSAGE = "Token not found";
    public const string TOKEN_REFRESH_FAILED_MESSAGE = "Token refresh failed";
    public const string TEST_TOKEN = "testToken";
}

public static class TestHelper
{
    public static Dictionary<string, StringValues> GetQueryParameters(string code) =>
        new() { { "code", code } };

    public static HttpContext GetHttpContext(Dictionary<string, StringValues> queryParameters)
    {
        var httpContext = new DefaultHttpContext();
        httpContext.Request.Query = new QueryCollection(queryParameters);
        return httpContext;
    }
}
