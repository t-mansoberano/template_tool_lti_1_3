using gec.Application.Contracts.Infrastructure.Lti;
using gec.Application.Contracts.Infrastructure.Lti.Models;
using gec.Application.Contracts.Server.Session;
using gec.Server.Common;
using Microsoft.AspNetCore.Mvc;

namespace gec.Server.Features.Lti;

[ApiController]
[Route("api/lti")]
public class LtiController : BaseController
{
    private readonly ILtiService _ltiService;
    private readonly ISessionStorageService _sessionStorageService;

    public LtiController(ILtiService ltiService, ISessionStorageService sessionStorageService)
    {
        _ltiService = ltiService;
        _sessionStorageService = sessionStorageService;
    }

    [HttpGet]
    public IActionResult Get()
    {
        var ltiContext = _sessionStorageService.Retrieve<LtiContext>("LtiContext");
        if (ltiContext.IsFailure)
            return Error(ltiContext.Error);

        return Ok(ltiContext.Value);
    }

    [HttpPost]
    public IActionResult LaunchLTI([FromForm] IFormCollection form)
    {
        var formModel = new LoginInitiationResponse(form.ToDictionary(x => x.Key, x => x.Value.ToString()));
        var redirectUrl = _ltiService.BuildAuthorizationUrl(formModel);
        if (redirectUrl.IsFailure)
            return Error(redirectUrl.Error);

        return Redirect(redirectUrl.Value);
    }

    [HttpPost]
    [Route("redirect")]
    public async Task<IActionResult> HandleRedirect([FromForm] IFormCollection form)
    {
        var context = await _ltiService.HandleRedirectAsync(form.ToDictionary(x => x.Key, x => x.Value.ToString()));
        if (context.IsFailure)
            return Error(context.Error);

        _sessionStorageService.Store("LtiContext", context.Value);

        return Redirect("/api/lti/oauth/token/validate");
    }

    [HttpGet]
    [Route("/api/.well-known/jwks.json")]
    public IActionResult GetJwks()
    {
        return Ok(_ltiService.GetJwks());
    }
}