namespace gec.Application.Contracts.Server.Configuration.Models;

public class CorsSettings
{
    public const string Key = "Cors";

    public string[] AllowedOrigins { get; set; } = Array.Empty<string>();
}