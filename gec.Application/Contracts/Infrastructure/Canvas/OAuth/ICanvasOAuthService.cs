using CSharpFunctionalExtensions;
using gec.Application.Contracts.Infrastructure.Canvas.OAuth.Models;

namespace gec.Application.Contracts.Infrastructure.Canvas.OAuth;

public interface ICanvasOAuthService
{
    string BuildAuthorizationUrl();
    Task<Result<CanvasAuthToken>> HandleOAuthCallbackAsync(Dictionary<string, string> query);
    Task<Result<CanvasAuthToken>> GetTokenAsync(CanvasAuthToken canvasAuthToken);
}