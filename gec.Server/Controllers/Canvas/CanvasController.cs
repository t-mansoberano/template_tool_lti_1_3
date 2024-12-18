using System.Text.Json;
using gec.Infrastructure.Canvas;
using gec.Server.Common;
using Microsoft.AspNetCore.Mvc;

namespace gec.Server.Controllers.Canvas;

[ApiController]
public class CanvasController : BaseController
{
    private readonly ICanvasOAuthService _canvasOAuthService;

    public CanvasController(ICanvasOAuthService canvasOAuthService)
    {
        _canvasOAuthService = canvasOAuthService;
    }   
    
    [HttpGet]
    [Route("api/lti/oauth/authorize")]
    public IActionResult AuthorizeUser()
    {
        var authorizationUrl = _canvasOAuthService.BuildAuthorizationUrl();
        return Redirect(authorizationUrl);
    }

    [HttpGet]
    [Route("api/lti/oauth/callback")]
    public async Task<IActionResult> HandleOAuthCallback()
    {
        var query = HttpContext.Request.Query.ToDictionary(x => x.Key, x => x.Value.ToString());
        var tokenResponse = await _canvasOAuthService.HandleOAuthCallbackAsync(query);
        if (tokenResponse.IsFailure) return Error(tokenResponse.Error);

        HttpContext.Session.SetString("tokenResponse", JsonSerializer.Serialize(tokenResponse.Value));
        return Redirect("/");
    }

}