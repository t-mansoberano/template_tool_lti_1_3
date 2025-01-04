namespace gec.Application.Contracts.Server.Configuration.Models;

public class SessionSettings
{
    public string CookieDomain { get; set; } = string.Empty;
    public int IdleTimeoutMinutes { get; set; } = 20;
}
