using gec.Application.Contracts.Server.Configuration;
using Serilog;

namespace gec.Server.Startup;

public static class LoggingConfiguration
{
    public static void ConfigureSerilog(IServiceCollection services, IAppSettingsService appSettings)
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File(appSettings.Logging.FilePath, rollingInterval: RollingInterval.Day)
            .WriteTo.Sentry(o =>
            {
                o.Dsn = appSettings.Sentry.Dsn;
                o.MinimumBreadcrumbLevel = Serilog.Events.LogEventLevel.Verbose;
                o.MinimumEventLevel = Serilog.Events.LogEventLevel.Verbose;
                o.AttachStacktrace = true;
                o.Release = appSettings.Sentry.Release;
                o.Environment = appSettings.Sentry.Environment;
                o.Debug = appSettings.Sentry.Debug;
                o.TracesSampleRate = appSettings.Sentry.TracesSampleRate;
            })
            .CreateLogger();
    }
}