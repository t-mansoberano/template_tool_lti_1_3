using gec.Application.Contracts.Infrastructure.Canvas.OAuth;
using gec.Application.Contracts.Infrastructure.Canvas.OAuth.Models;
using gec.Application.Contracts.Infrastructure.Lti.Models;
using gec.Application.Contracts.Server.Configuration;
using gec.Application.Contracts.Server.Fake;
using gec.Application.Contracts.Server.Session;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace gec.Server.Common;

public class ValidateCanvasTokenAttribute : ActionFilterAttribute
{
    private readonly ICanvasOAuthService _canvasOAuthService;
    private readonly ISessionStorageService _sessionStorageService;
    private readonly IAppSettingsService _appSettingsService;
    private readonly IFakeDataService _fakeDataService;

    public ValidateCanvasTokenAttribute(ICanvasOAuthService canvasOAuthService,
        ISessionStorageService sessionStorageService,
        IAppSettingsService appSettingsService,
        IFakeDataService fakeDataService)
    {
        _canvasOAuthService = canvasOAuthService;
        _sessionStorageService = sessionStorageService;
        _appSettingsService = appSettingsService;
        _fakeDataService = fakeDataService;
    }

    public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (_appSettingsService.Fake.UseFakeLti)
        {
            HandleFakeToken();         
        }
        else
        {
            var canvasAuthToken = _sessionStorageService.Retrieve<CanvasAuthToken>(CanvasAuthToken.Key);
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
            
            _sessionStorageService.Store(CanvasAuthToken.Key, refreshedToken.Value);
        }
        
        await next(); // Continúa con la acción del controlador
    }
    
    private void HandleFakeToken()
    {
        // Se obtiene y almacena el fake LtiContext
        var fakeLtiContextResult = _fakeDataService.GetFakeData<LtiContext>(_appSettingsService.Fake.FakeLtiContextPath);
        if (fakeLtiContextResult.IsSuccess)
        {
            _sessionStorageService.Store(LtiContext.Key, fakeLtiContextResult.Value);
        }

        // Se crea y almacena el token fake de Canvas
        var fakeCanvasAuthToken = new CanvasAuthToken
        {
            AccessToken = _appSettingsService.Fake.FakeApiCanvasToken,
            ExpiresIn = 3600,
            TokenType = "Bearer",
            RefreshToken = _appSettingsService.Fake.FakeApiCanvasToken,
            CanvasRegion = "https://canvas.instructure.com",
            User = new UserInfo(),
            ExpirationTime = DateTime.Now.AddHours(1)
        };

        _sessionStorageService.Store(CanvasAuthToken.Key, fakeCanvasAuthToken);
    }    
}