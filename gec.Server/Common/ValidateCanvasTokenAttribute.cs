using gec.Application.Contracts.Infrastructure.Canvas;
using gec.Application.Contracts.Infrastructure.Canvas.Models;
using gec.Application.Contracts.Server;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace gec.Server.Common;

public class ValidateCanvasTokenAttribute: ActionFilterAttribute
{
    private readonly ICanvasOAuthService _canvasOAuthService;
    private readonly ISessionStorageService _sessionStorageService;

    public ValidateCanvasTokenAttribute(ICanvasOAuthService canvasOAuthService, ISessionStorageService sessionStorageService)
    {
        _canvasOAuthService = canvasOAuthService;
        _sessionStorageService = sessionStorageService;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var canvasAuthToken = _sessionStorageService.Retrieve<CanvasAuthToken>("CanvasAuthToken");
        if (canvasAuthToken.IsFailure)
        {
            context.Result = new RedirectResult(_canvasOAuthService.BuildAuthorizationUrl());
            return;
        }

        var refreshedToken = await _canvasOAuthService.GetTokenAsync(canvasAuthToken.Value);
        if (refreshedToken.IsFailure)
        {
            context.Result = new RedirectResult(_canvasOAuthService.BuildAuthorizationUrl());
            return;
        }

        _sessionStorageService.Store("CanvasAuthToken", refreshedToken.Value);

        await next(); // Continúa con la acción del controlador
    }
}