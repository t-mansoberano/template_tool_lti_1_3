using gec.Server.Common;

namespace gec.Server;

public static class ServiceRegistration
{
    public static IServiceCollection AddServerServices(this IServiceCollection services)
    {
        services.AddScoped<ISessionStorageService, SessionStorageService>();

        return services;
    }
}