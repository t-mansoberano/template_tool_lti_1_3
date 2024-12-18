namespace gec.Infrastructure.Canvas;

public class TokenResponse
{
    public string AccessToken { get; set; }
    public string TokenType { get; set; }
    public int ExpiresIn { get; set; }
    public string RefreshToken { get; set; }
    public UserInfo User { get; set; }
    public string CanvasRegion { get; set; }
}

public class UserInfo
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string GlobalId { get; set; }
    public string EffectiveLocale { get; set; }
}
