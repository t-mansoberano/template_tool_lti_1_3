using gec.Application.Contracts.Infrastructure.Lti;
using gec.Application.Contracts.Infrastructure.Lti.Models;
using gec.Application.Contracts.Server.Session;
using gec.Server.Integrations.Lti;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace gec.Test.Server.Integrations;

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
        _mockSessionStorageService
            .Setup(s => s.Retrieve<LtiContext>("LtiContext"))
            .Returns(CSharpFunctionalExtensions.Result.Failure<LtiContext>("Session not found"));

        // Act
        var result = _controller.Get();

        // Assert
        Assert.IsInstanceOf<ObjectResult>(result);
        var objectResult = result as ObjectResult;
        Assert.AreEqual(400, objectResult.StatusCode);
        Assert.AreEqual("Session not found", ((gec.Server.Common.Envelope)objectResult.Value).ErrorMessage);
    }

    [Test]
    public void Get_ShouldReturnOk_WhenSessionContextExists()
    {
        // Arrange
        var ltiContext = new LtiContext();
        _mockSessionStorageService
            .Setup(s => s.Retrieve<LtiContext>("LtiContext"))
            .Returns(CSharpFunctionalExtensions.Result.Success(ltiContext));

        // Act
        var result = _controller.Get();

        // Assert
        Assert.IsInstanceOf<OkObjectResult>(result);
        var okResult = result as OkObjectResult;
        Assert.AreSame(ltiContext, ((gec.Server.Common.Envelope<LtiContext>)okResult.Value).Result);
    }

    [Test]
    public void LaunchLTI_ShouldReturnError_WhenAuthorizationUrlFails()
    {
        // Arrange
        var form = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>());
        _mockLtiService
            .Setup(s => s.BuildAuthorizationUrl(It.IsAny<LoginInitiationResponse>()))
            .Returns(CSharpFunctionalExtensions.Result.Failure<string>("Invalid form data"));

        // Act
        var result = _controller.LaunchLTI(form);

        // Assert
        Assert.IsInstanceOf<ObjectResult>(result);
        var objectResult = result as ObjectResult;
        Assert.AreEqual(400, objectResult.StatusCode);
        Assert.AreEqual("Invalid form data", ((gec.Server.Common.Envelope)objectResult.Value).ErrorMessage);
    }

    [Test]
    public void LaunchLTI_ShouldRedirect_WhenAuthorizationUrlIsSuccessful()
    {
        // Arrange
        var form = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>());
        _mockLtiService
            .Setup(s => s.BuildAuthorizationUrl(It.IsAny<LoginInitiationResponse>()))
            .Returns(CSharpFunctionalExtensions.Result.Success("http://example.com"));

        // Act
        var result = _controller.LaunchLTI(form);

        // Assert
        Assert.IsInstanceOf<RedirectResult>(result);
        var redirectResult = result as RedirectResult;
        Assert.AreEqual("http://example.com", redirectResult.Url);
    }

    [Test]
    public async Task HandleRedirect_ShouldReturnError_WhenRedirectFails()
    {
        // Arrange
        var form = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>());
        _mockLtiService
            .Setup(s => s.HandleRedirectAsync(It.IsAny<Dictionary<string, string>>()))
            .ReturnsAsync(CSharpFunctionalExtensions.Result.Failure<LtiContext>("Redirect failed"));

        // Act
        var result = await _controller.HandleRedirect(form);

        // Assert
        Assert.IsInstanceOf<ObjectResult>(result);
        var objectResult = result as ObjectResult;
        Assert.AreEqual(400, objectResult.StatusCode);
        Assert.AreEqual("Redirect failed", ((gec.Server.Common.Envelope)objectResult.Value).ErrorMessage);
    }

    [Test]
    public async Task HandleRedirect_ShouldStoreContextAndRedirect_WhenSuccessful()
    {
        // Arrange
        var form = new FormCollection(new Dictionary<string, Microsoft.Extensions.Primitives.StringValues>());
        var ltiContext = new LtiContext();
        _mockLtiService
            .Setup(s => s.HandleRedirectAsync(It.IsAny<Dictionary<string, string>>()))
            .ReturnsAsync(CSharpFunctionalExtensions.Result.Success(ltiContext));

        // Act
        var result = await _controller.HandleRedirect(form);

        // Assert
        _mockSessionStorageService.Verify(s => s.Store("LtiContext", ltiContext), Times.Once);
        Assert.IsInstanceOf<RedirectResult>(result);
        var redirectResult = result as RedirectResult;
        Assert.AreEqual("/api/lti/oauth/token/validate", redirectResult.Url);
    }

    [Test]
    public void GetJwks_ShouldReturnKeys()
    {
        // Arrange
        var keys = new object();
        _mockLtiService
            .Setup(s => s.GetJwks())
            .Returns(CSharpFunctionalExtensions.Result.Success(keys.ToString()));

        // Act
        var result = _controller.GetJwks();

        // Assert
        Assert.IsInstanceOf<OkObjectResult>(result);
        var okResult = result as OkObjectResult;

        // Verificar que el valor es un Envelope genérico con el tipo correcto
        Assert.IsInstanceOf(typeof(gec.Server.Common.Envelope<CSharpFunctionalExtensions.Result<string>>), okResult.Value);
        var envelope = okResult.Value as gec.Server.Common.Envelope<CSharpFunctionalExtensions.Result<string>>;

        // Verificar que el contenido del Envelope coincide con lo esperado
        Assert.IsTrue(envelope.Result.IsSuccess);
        Assert.AreEqual(keys.ToString(), envelope.Result.Value);
    }


}
