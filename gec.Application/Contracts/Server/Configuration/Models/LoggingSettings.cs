namespace gec.Application.Contracts.Server.Configuration.Models;

public class LoggingSettings
{
    public const string Key = "Logging";

    public string FilePath { get; set; } = string.Empty;
}