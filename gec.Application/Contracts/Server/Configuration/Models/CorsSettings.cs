namespace gec.Application.Contracts.Server.Configuration.Models;

public class CorsSettings
{
    public string[] AllowedOrigins { get; set; } = Array.Empty<string>();
}
