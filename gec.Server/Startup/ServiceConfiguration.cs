using gec.Application;
using gec.Infrastructure;

namespace gec.Server.Startup;

public static class ServiceConfiguration
{
    public static void AddCustomServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddApplicationServices();
        services.AddInfrastructureServices(configuration);

        services.AddDistributedMemoryCache();
        services.AddSession(options =>
        {
            options.Cookie.Domain = "manuelsoberano.ngrok.dev";
            options.IdleTimeout = TimeSpan.FromMinutes(480);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
            options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
            options.Cookie.SameSite = SameSiteMode.None;
        });

        services.AddHttpContextAccessor();

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(policy =>
                policy.WithOrigins("https://localhost:4200",
                        "https://localhost:7051",
                        "http://localhost:5156",
                        "https://manuelsoberano.ngrok.dev")
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
