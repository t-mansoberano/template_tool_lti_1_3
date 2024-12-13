using gec.Infrastructure.Lti;
using Microsoft.Extensions.DependencyInjection;

namespace gec.Infrastructure;

public static class ServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services)
    {
        services.AddScoped<ILtiService, LtiService>();

        return services;
    }
}