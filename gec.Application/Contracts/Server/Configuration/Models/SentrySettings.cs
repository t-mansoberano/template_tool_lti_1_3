namespace gec.Application.Contracts.Server.Configuration.Models;

public class SentrySettings
{
    public string Dsn { get; set; } = string.Empty;
    public bool Debug { get; set; }
    public double TracesSampleRate { get; set; }
    public string Environment { get; set; } = string.Empty;
    public string Release { get; set; } = string.Empty;
}
