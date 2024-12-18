using gec.Infrastructure.Canvas;
using gec.Infrastructure.Lti;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace gec.Infrastructure;

public static class ServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        // Servicios relacionados con LTI
        services.AddScoped<ILtiService, LtiService>();
        
        // Servicios relacionados con Canvas
        services.AddHttpClient("CanvasClient", client =>
        {
            client.BaseAddress = new Uri(configuration["Canvas:ApiBaseUrl"]);
        });
        services.AddSingleton<ICanvasOAuthService, CanvasOAuthService>();
        services.AddScoped<ICanvasService, CanvasService>();

        return services;
    }
}