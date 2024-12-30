using gec.Application.Contracts.Infrastructure.Canvas;
using gec.Application.Contracts.Infrastructure.Canvas.Models;
using gec.Application.Contracts.Server;
using gec.Server.Common;
using Microsoft.AspNetCore.Mvc;

namespace gec.Server.Features.Canvas;

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

        _sessionStorageService.Store("CanvasAuthToken", tokenResponse.Value);

        return Redirect("/");
    }

    [HttpGet]
    [Route("api/lti/oauth/token/validate")]
    public async Task<IActionResult> ValidateOrRefreshToken()
    {
        var canvasAuthToken = _sessionStorageService.Retrieve<CanvasAuthToken>("CanvasAuthToken");
        if (canvasAuthToken.IsFailure)
            return Redirect(_canvasOAuthService.BuildAuthorizationUrl());

        canvasAuthToken = await _canvasOAuthService.GetTokenAsync(canvasAuthToken.Value);
        if (canvasAuthToken.IsFailure) return Redirect(_canvasOAuthService.BuildAuthorizationUrl());
        
        _sessionStorageService.Store("CanvasAuthToken", canvasAuthToken.Value);

        return Redirect("/");
    }
}