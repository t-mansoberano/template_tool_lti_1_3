using gec.Application.Contracts.Server.Configuration;
using gec.Application.Contracts.Server.Configuration.Models;
using Microsoft.Extensions.Options;

namespace gec.Server.Common;

public class AppSettingsService : IAppSettingsService
{
    public CanvasSettings Canvas { get; }
    public LtiSettings Lti { get; }
    public CorsSettings Cors { get; }
    public SentrySettings Sentry { get; }
    public LoggingSettings Logging { get; }
    public SessionSettings Session { get; }

    public AppSettingsService(IOptions<CanvasSettings> canvasSettings, IOptions<LtiSettings> ltiSettings,
        IOptions<CorsSettings> corsSettings, IOptions<SentrySettings> sentrySettings,
        IOptions<LoggingSettings> loggingSettings, IOptions<SessionSettings> sessionSettings)
    {
        Canvas = canvasSettings.Value;
        Lti = ltiSettings.Value;
        Cors = corsSettings.Value;
        Sentry = sentrySettings.Value;
        Logging = loggingSettings.Value;
        Session = sessionSettings.Value;
    }
}