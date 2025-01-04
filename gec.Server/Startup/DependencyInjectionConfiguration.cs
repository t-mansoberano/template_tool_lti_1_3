using gec.Application.Contracts.Server.Configuration;

namespace gec.Server.Startup;

public static class ServiceConfiguration
{
    public static void AddCustomServices(this IServiceCollection services, IAppSettingsService appSettings)
    {
        services.AddDistributedMemoryCache();
        services.AddSession(options =>
        {
            options.Cookie.Domain = appSettings.Session.CookieDomain;
            options.IdleTimeout = TimeSpan.FromMinutes(appSettings.Session.IdleTimeoutMinutes);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.None;
        });

        services.AddHttpContextAccessor();

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
                policy.WithOrigins(appSettings.Cors.AllowedOrigins)
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    .AllowCredentials());
        });

        services.AddHttpClient();
        services.AddControllers();
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen();
    }
}
