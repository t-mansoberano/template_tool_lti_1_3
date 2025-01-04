using gec.Application.Contracts.Server.Configuration.Models;

namespace gec.Application.Contracts.Server.Configuration;

public interface IAppSettingsService
{
    CanvasSettings Canvas { get; }
    LtiSettings Lti { get; }
    CorsSettings Cors { get; }
    SentrySettings Sentry { get; }
    LoggingSettings Logging { get; }
    SessionSettings Session { get; }
}