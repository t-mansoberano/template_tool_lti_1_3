namespace gec.Application.Contracts.Infrastructure.Canvas.Models;

public class CanvasAuthToken
{
    public string AccessToken { get; set; } = null!;
    public string TokenType { get; set; } = null!;
    public int ExpiresIn { get; set; }
    public string RefreshToken { get; set; } = null!;
    public UserInfo User { get; set; } = null!;
    public string CanvasRegion { get; set; } = null!;
    public DateTime? ExpirationTime { get; set; }
    
    public void CalculateExpirationTime()
    {
        if (ExpiresIn > 0)
        {
            ExpirationTime = DateTime.UtcNow.AddSeconds(ExpiresIn);
        }
    }

    public void SetRefreshToken(string refreshToken)
    {
        RefreshToken = refreshToken;
    }

    public bool IsValid()
    {
        return !string.IsNullOrEmpty(AccessToken) &&
               ExpirationTime.HasValue &&
               DateTime.UtcNow < ExpirationTime.Value;
    }  
}

public class UserInfo
{
    public int Id { get; set; }
    public string Name { get; set; } = null!;
    public string GlobalId { get; set; } = null!;
    public string EffectiveLocale { get; set; } = null!;
}
