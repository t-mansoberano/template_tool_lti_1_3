using CSharpFunctionalExtensions;
using gec.Application.Contracts.Infrastructure.Canvas.Models;

namespace gec.Application.Contracts.Infrastructure.Canvas;

public interface ICanvasOAuthService
{
    string BuildAuthorizationUrl();
    Task<Result<CanvasAuthToken>> HandleOAuthCallbackAsync(Dictionary<string, string> query);
    Task<Result<CanvasAuthToken>> GetTokenAsync(CanvasAuthToken canvasAuthToken);
}