namespace gec.Application.Contracts.Server.Configuration.Models;

public class SessionSettings
{
    public const string Key = "Session";

    public string CookieDomain { get; set; } = string.Empty;
    public int IdleTimeoutMinutes { get; set; } = 20;
}
