using Serilog;

namespace gec.Server.Startup;

public static class LoggingConfiguration
{
    public static void ConfigureSerilog()
    {
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console()
            .WriteTo.File(@"C:\Log\GestorEvaluacionesCompetencia-.log", rollingInterval: RollingInterval.Day)
            .WriteTo.Sentry(o =>
            {
                o.Dsn =
                    "https://70801557168fc87f254a77ff783afb6c@o4508458878500864.ingest.us.sentry.io/4508458976870400";
                o.MinimumBreadcrumbLevel = Serilog.Events.LogEventLevel.Verbose;
                o.MinimumEventLevel = Serilog.Events.LogEventLevel.Verbose;
                o.AttachStacktrace = true;
                o.Release = "gec@0.0.1";
                o.Environment = "Development";
                o.Debug = true;
                o.TracesSampleRate = 1.0;
            })
            .CreateLogger();
    }
}