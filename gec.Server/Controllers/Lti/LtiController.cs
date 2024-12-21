using System.Text.Json;
using gec.Infrastructure.Lti;
using gec.Infrastructure.Lti.Models;
using gec.Server.Common;
using Microsoft.AspNetCore.Mvc;

namespace gec.Server.Controllers.Lti;

[ApiController]
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
    [Route("api/lti")]
    public IActionResult Get()
    {
        var resourceContext = _sessionStorageService.Retrieve<ResourceContext>("ResourceContext");

        if (resourceContext == null)
        {
            return Error("No hay datos guardados en el sistema.");
        }

        return Ok(resourceContext);
    }
    
    [HttpPost]
    [Route("api/lti")]
    public IActionResult LaunchLTI([FromForm] IFormCollection form)
    {
        var formModel = new LoginInitiationResponse(form.ToDictionary(x => x.Key, x => x.Value.ToString()));
        var redirectUrl = _ltiService.BuildAuthorizationUrl(formModel);
        if (redirectUrl.IsFailure) return Error(redirectUrl.Error);
        
        return Redirect(redirectUrl.Value);
    }

    [HttpPost]
    [Route("api/lti/redirect")]
    public async Task<IActionResult> HandleRedirect([FromForm] IFormCollection form)
    {
        var context = await _ltiService.HandleRedirectAsync(form.ToDictionary(x => x.Key, x => x.Value.ToString()));
        if (context.IsFailure) return Error(context.Error);
        
        _sessionStorageService.Store("ResourceContext", context.Value);

        return Redirect("/api/lti/oauth/token/validate");        
    }
    
    [HttpGet]
    [Route("api/.well-known/jwks.json")]
    public IActionResult GetJwks()
    {
        return Ok(_ltiService.GetJwks());
    }
}
