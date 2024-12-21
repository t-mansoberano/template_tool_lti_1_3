using System.Text.Json;
using gec.Infrastructure.Canvas;
using gec.Server.Common;
using Microsoft.AspNetCore.Mvc;

namespace gec.Server.Controllers.Canvas;

[ApiController]
public class CanvasController : BaseController
{
    private readonly ICanvasOAuthService _canvasOAuthService;
    private readonly ISessionStorageService _sessionStorageService;

    public CanvasController(ICanvasOAuthService canvasOAuthService, ISessionStorageService sessionStorageService)
    {
        _canvasOAuthService = canvasOAuthService;
        _sessionStorageService = sessionStorageService;
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

        _sessionStorageService.Store("CanvasToken", tokenResponse.Value);

        return Redirect("/");
    }

    [HttpGet]
    [Route("api/lti/oauth/token/validate")]
    public async Task<IActionResult> ValidateOrRefreshToken()
    {
        var tokenResponse = _sessionStorageService.Retrieve<TokenResponse>("CanvasToken");
        if (tokenResponse == null)
        {
            return Redirect(_canvasOAuthService.BuildAuthorizationUrl());
        }

        var token = await _canvasOAuthService.GetTokenAsync(tokenResponse);
        if (token.IsFailure) return Redirect(_canvasOAuthService.BuildAuthorizationUrl());
        
        _sessionStorageService.Store("CanvasToken", token.Value);

        return Redirect("/");
    }
}