using CSharpFunctionalExtensions;
using FluentAssertions;
using gec.Application.Contracts.Infrastructure.Lti;
using gec.Application.Contracts.Infrastructure.Lti.Models;
using gec.Application.Contracts.Server.Session;
using gec.Server.Common;
using gec.Server.Integrations.Lti;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Moq;

namespace gec.Test.Server.Integrations.Lti;

[TestFixture]
public class LtiControllerTests
{
    private Mock<ILtiService> _mockLtiService;
    private Mock<ISessionStorageService> _mockSessionStorageService;
    private LtiController _controller;

    [SetUp]
    public void SetUp()
    {
        _mockLtiService = new Mock<ILtiService>();
        _mockSessionStorageService = new Mock<ISessionStorageService>();
        _controller = new LtiController(_mockLtiService.Object, _mockSessionStorageService.Object);
    }

    [Test]
    public void Get_ShouldReturnError_WhenSessionContextIsFailure()
    {
        // Arrange
        const string errorMessage = TestConstants.SESSION_NOT_FOUND;
        _mockSessionStorageService
            .Setup(s => s.Retrieve<LtiContext>("LtiContext"))
            .Returns(Result.Failure<LtiContext>(errorMessage));

        // Act
        var result = _controller.Get();

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.StatusCode.Should().Be(400);
        ((Envelope)badRequestResult.Value).ErrorMessage.Should().Be(errorMessage);
    }

    [Test]
    public void Get_ShouldReturnOk_WhenSessionContextExists()
    {
        // Arrange
        var ltiContext = TestHelper.GetSampleLtiContext();
        _mockSessionStorageService
            .Setup(s => s.Retrieve<LtiContext>("LtiContext"))
            .Returns(Result.Success(ltiContext));

        // Act
        var result = _controller.Get();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;
        ((Envelope<LtiContext>)okResult.Value).Result.Should().BeEquivalentTo(ltiContext);
    }

    [Test]
    public void LaunchLTI_ShouldReturnError_WhenAuthorizationUrlFails()
    {
        // Arrange
        const string errorMessage = TestConstants.INVALID_FORM_DATA;
        var form = TestHelper.GetEmptyFormCollection();
        _mockLtiService
            .Setup(s => s.BuildAuthorizationUrl(It.IsAny<LoginInitiationResponse>()))
            .Returns(Result.Failure<string>(errorMessage));

        // Act
        var result = _controller.LaunchLTI(form);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.StatusCode.Should().Be(400);
        ((Envelope)badRequestResult.Value).ErrorMessage.Should().Be(errorMessage);
    }

    [Test]
    public void LaunchLTI_ShouldRedirect_WhenAuthorizationUrlIsSuccessful()
    {
        // Arrange
        const string authorizationUrl = TestConstants.AUTHORIZATION_URL;
        var form = TestHelper.GetEmptyFormCollection();
        _mockLtiService
            .Setup(s => s.BuildAuthorizationUrl(It.IsAny<LoginInitiationResponse>()))
            .Returns(Result.Success(authorizationUrl));

        // Act
        var result = _controller.LaunchLTI(form);

        // Assert
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Url.Should().Be(authorizationUrl);
    }

    [Test]
    public async Task HandleRedirect_ShouldReturnError_WhenRedirectFails()
    {
        // Arrange
        const string errorMessage = TestConstants.REDIRECT_FAILED;
        var form = TestHelper.GetEmptyFormCollection();
        _mockLtiService
            .Setup(s => s.HandleRedirectAsync(It.IsAny<Dictionary<string, string>>()))
            .ReturnsAsync(Result.Failure<LtiContext>(errorMessage));

        // Act
        var result = await _controller.HandleRedirect(form);

        // Assert
        result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result as BadRequestObjectResult;
        badRequestResult.StatusCode.Should().Be(400);
        ((Envelope)badRequestResult.Value).ErrorMessage.Should().Be(errorMessage);
    }

    [Test]
    public async Task HandleRedirect_ShouldStoreContextAndRedirect_WhenSuccessful()
    {
        // Arrange
        const string redirectUrl = TestConstants.REDIRECT_URL;
        var ltiContext = TestHelper.GetSampleLtiContext();
        var form = TestHelper.GetEmptyFormCollection();
        _mockLtiService
            .Setup(s => s.HandleRedirectAsync(It.IsAny<Dictionary<string, string>>()))
            .ReturnsAsync(Result.Success(ltiContext));

        // Act
        var result = await _controller.HandleRedirect(form);

        // Assert
        _mockSessionStorageService.Verify(s => s.Store("LtiContext", ltiContext), Times.Once);
        result.Should().BeOfType<RedirectResult>();
        var redirectResult = result as RedirectResult;
        redirectResult.Url.Should().Be(redirectUrl);
    }

    [Test]
    public void GetJwks_ShouldReturnKeys()
    {
        // Arrange
        var expectedKeys = TestHelper.GetSampleKeys();
        _mockLtiService
            .Setup(s => s.GetJwks())
            .Returns(Result.Success(expectedKeys));

        // Act
        var result = _controller.GetJwks();

        // Assert
        result.Should().BeOfType<OkObjectResult>();
        var okResult = result as OkObjectResult;

        var envelope = okResult.Value as Envelope<Result<string>>;
        envelope.Should().NotBeNull();
        envelope.Result.IsSuccess.Should().BeTrue();
        envelope.Result.Value.Should().Be(expectedKeys);
    }

}

public static class TestConstants
{
    public const string SESSION_NOT_FOUND = "Session not found";
    public const string INVALID_FORM_DATA = "Invalid form data";
    public const string REDIRECT_FAILED = "Redirect failed";
    public const string AUTHORIZATION_URL = "http://example.com";
    public const string REDIRECT_URL = "/api/lti/oauth/token/validate";
}

public static class TestHelper
{
    public static FormCollection GetEmptyFormCollection() =>
        new(new Dictionary<string, StringValues>());

    public static LtiContext GetSampleLtiContext() =>
        new();

    public static string GetSampleKeys() => "SampleKeys";
}
