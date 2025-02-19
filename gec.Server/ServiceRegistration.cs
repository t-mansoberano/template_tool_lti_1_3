using gec.Application.Contracts.Server.Configuration;
using gec.Application.Contracts.Server.Configuration.Models;
using gec.Application.Contracts.Server.Fake;
using gec.Application.Contracts.Server.Session;
using gec.Server.Common;

namespace gec.Server;

public static class ServiceRegistration
{
    public static IServiceCollection AddServerServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<LoggingSettings>(configuration.GetSection(LoggingSettings.Key));
        services.Configure<SessionSettings>(configuration.GetSection(SessionSettings.Key));
        services.Configure<CanvasSettings>(configuration.GetSection(CanvasSettings.Key));
        services.Configure<LtiSettings>(configuration.GetSection(LtiSettings.Key));
        services.Configure<CorsSettings>(configuration.GetSection(CorsSettings.Key));
        services.Configure<SentrySettings>(configuration.GetSection(SentrySettings.Key));
        services.Configure<FakeSettings>(configuration.GetSection(FakeSettings.Key));

        services.AddSingleton<IAppSettingsService, AppSettingsService>();
        services.AddSingleton<IFakeDataService, FakeDataService>();

        services.AddScoped<ISessionStorageService, SessionStorageService>();

        services.AddScoped<ValidateCanvasTokenAttribute>();

        return services;
    }
}