using gec.Application;
using gec.Infrastructure;

namespace gec.Server.Startup;

public static class LayerServiceRegistration
{
    public static void AddLayerServices(this IServiceCollection services)
    {
        services.AddApplicationServices();
        services.AddInfrastructureServices();
    }
}