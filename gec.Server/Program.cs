using gec.Server.Startup;

namespace gec.Server;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configurar fuentes de configuración
        builder.Configuration.AddConfigurationSources(builder.Environment);

        // Configurar servicios de servidor
        builder.Services.AddServerServices(builder.Configuration);
        builder.Services.AddLayerServices();

        builder.Services.AddCustomServices(builder.Configuration);

        // Configurar un logger
        LoggingConfiguration.ConfigureSerilog(builder.Configuration);

        // Configurar Sentry
        builder.WebHost.ConfigureSentry(builder.Configuration);

        var app = builder.Build();

        // Configurar middleware
        app.UseCustomMiddleware();

        app.Run();
    }
}