using CSharpFunctionalExtensions;

namespace gec.Infrastructure.Canvas;

public interface ICanvasOAuthService
{
    string BuildAuthorizationUrl();
    Task<Result<TokenResponse>> HandleOAuthCallbackAsync(Dictionary<string, string> query);
    Task<Result<TokenResponse>> GetTokenAsync(TokenResponse tokenResponse);
}