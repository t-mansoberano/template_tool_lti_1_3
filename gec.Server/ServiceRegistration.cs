using gec.Application.Contracts.Server;
using gec.Application.Contracts.Server.Configuration;
using gec.Application.Contracts.Server.Configuration.Models;
using gec.Server.Common;

namespace gec.Server;

public static class ServiceRegistration
{
    public static IServiceCollection AddServerServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<LoggingSettings>(configuration.GetSection("Logging"));
        services.Configure<SessionSettings>(configuration.GetSection("Session"));
        services.Configure<CanvasSettings>(configuration.GetSection("Canvas"));
        services.Configure<LtiSettings>(configuration.GetSection("Lti"));
        services.Configure<CorsSettings>(configuration.GetSection("Cors"));
        services.Configure<SentrySettings>(configuration.GetSection("Sentry"));

        services.AddSingleton<IAppSettingsService, AppSettingsService>();

        services.AddScoped<ISessionStorageService, SessionStorageService>();
        
        services.AddScoped<ValidateCanvasTokenAttribute>();

        return services;
    }
}