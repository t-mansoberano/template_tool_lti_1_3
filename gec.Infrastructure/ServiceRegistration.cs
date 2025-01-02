using gec.Application.Contracts.Infrastructure.Canvas;
using gec.Application.Contracts.Infrastructure.Canvas.Enrollments;
using gec.Application.Contracts.Infrastructure.Federation;
using gec.Application.Contracts.Infrastructure.Lti;
using gec.Infrastructure.Canvas;
using gec.Infrastructure.Canvas.Enrollments;
using gec.Infrastructure.Common;
using gec.Infrastructure.Federation;
using gec.Infrastructure.Lti;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace gec.Infrastructure;

public static class ServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddSingleton<AppSettingsService>();

        // Servicios relacionados con LTI
        services.AddScoped<ILtiService, LtiService>();
        services.AddScoped<IJwtValidationService, JwtValidationService>();

        // Servicios relacionados con Canvas
        services.AddHttpClient("CanvasClient", (provider, client) =>
        {
            var appSettings = provider.GetRequiredService<AppSettingsService>();
            client.BaseAddress = new Uri(appSettings.CanvasBaseUrl);
        });
        services.AddScoped<ICanvasOAuthService, CanvasOAuthService>();
        services.AddScoped<ICanvasApiClient, CanvasApiClient>();
        
        services.AddScoped<IEnrollmentsService, EnrollmentsService>();
        
        // Servicios relacionados con la Federación
        services.AddScoped<IFederationService, FederationService>();
        
        return services;
    }
}