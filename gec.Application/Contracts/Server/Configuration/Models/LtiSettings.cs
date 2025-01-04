namespace gec.Application.Contracts.Server.Configuration.Models;

public class LtiSettings
{
    public string UrlBase { get; set; } = string.Empty;
    public string ValidIssuer { get; set; } = string.Empty;
    public string RedirectUri { get; set; } = string.Empty;
    public string ClientId { get; set; } = string.Empty;
    public string ClientSecret { get; set; } = string.Empty;
}