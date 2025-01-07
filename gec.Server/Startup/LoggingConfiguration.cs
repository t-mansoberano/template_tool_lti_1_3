using gec.Application.Contracts.Server.Configuration.Models;
using Serilog;

namespace gec.Server.Startup;

public static class LoggingConfiguration
{
    public static void ConfigureSerilog(IConfiguration configuration)
    {
        var loggingSettings = configuration.GetSection(LoggingSettings.Key).Get<LoggingSettings>()!;
        var sentrySettings  = configuration.GetSection(SentrySettings.Key).Get<SentrySettings>()!;
        
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File(loggingSettings.FilePath, rollingInterval: RollingInterval.Day)
            .WriteTo.Sentry(o =>
            {
                o.Dsn = sentrySettings.Dsn;
                o.MinimumBreadcrumbLevel = Serilog.Events.LogEventLevel.Verbose;
                o.MinimumEventLevel = Serilog.Events.LogEventLevel.Verbose;
                o.AttachStacktrace = true;
                o.Release = sentrySettings.Release;
                o.Environment = sentrySettings.Environment;
                o.Debug = sentrySettings.Debug;
                o.TracesSampleRate = sentrySettings.TracesSampleRate;
            })
            .CreateLogger();
    }
}