using gec.Application.Contracts.Server.Configuration;
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

        var appSettings = builder.Services.BuildServiceProvider().GetRequiredService<IAppSettingsService>();

        builder.Services.AddCustomServices(appSettings);

        // Configurar un logger
        LoggingConfiguration.ConfigureSerilog(builder.Services, appSettings);

        // Configurar Sentry
        builder.WebHost.ConfigureSentry(appSettings);

        var app = builder.Build();

        // Configurar middleware
        app.UseCustomMiddleware();

        app.Run();
    }
}