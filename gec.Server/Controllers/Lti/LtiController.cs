using System.Text.Json;
using System.Text.Json.Serialization;
using gec.Infrastructure.Lti;
using gec.Infrastructure.Lti.Models;
using gec.Server.Common;
using Microsoft.AspNetCore.Mvc;

namespace gec.Server.Controllers.Lti;

[ApiController]
public class LtiController : BaseController
{
    private readonly ILtiService _ltiService;
    private readonly IConfiguration _configuration;

    public LtiController(ILtiService ltiService, IConfiguration configuration)
    {
        _ltiService = ltiService;
        _configuration = configuration;
    }

    [HttpGet]
    [Route("api/lti")]
    public Task<IActionResult> Get()
    {
        var resourceContextJson = HttpContext.Session.GetString("resourceContext");

        if (string.IsNullOrEmpty(resourceContextJson))
        {
            return Task.FromResult(Error("No hay datos guardados en la sesión."));
        }

        var resourceContext = JsonSerializer.Deserialize<ResourceContext>(resourceContextJson);

        return Task.FromResult(Ok(resourceContext));
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
        
        HttpContext.Session.SetString("resourceContext", JsonSerializer.Serialize(context.Value));

        return Redirect("/api/lti/oauth/token/validate");        
    }
    
    [HttpGet]
    [Route("api/.well-known/jwks.json")]
    public IActionResult GetJwks()
    {
        return Ok(_ltiService.GetJwks());
    }
}
