using gec.Application.Contracts.Server.Configuration.Models;

namespace gec.Server.Startup;

public static class ServiceConfiguration
{
    public static void AddCustomServices(this IServiceCollection services, IConfiguration configuration)
    {
        var sessionSettings = configuration.GetSection(SessionSettings.Key).Get<SessionSettings>()!;
        
        services.AddDistributedMemoryCache();
        services.AddSession(options =>
        {
            options.Cookie.Domain = sessionSettings.CookieDomain;
            options.IdleTimeout = TimeSpan.FromMinutes(sessionSettings.IdleTimeoutMinutes);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.None;
        });

        services.AddHttpContextAccessor();

        var corsSettings = configuration.GetSection(CorsSettings.Key).Get<CorsSettings>()!;
        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
                policy.WithOrigins(corsSettings.AllowedOrigins)
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
