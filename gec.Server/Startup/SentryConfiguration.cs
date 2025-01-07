using gec.Application.Contracts.Server.Configuration.Models;

namespace gec.Server.Startup;

public static class SentryConfiguration
{
    public static void ConfigureSentry(this IWebHostBuilder webHostBuilder, IConfiguration configuration)
    {
        var sentrySettings = configuration.GetSection(SentrySettings.Key).Get<SentrySettings>()!;
        
        webHostBuilder.UseSentry(o =>
        {
            o.Dsn = sentrySettings.Dsn;
            o.Debug = sentrySettings.Debug; // Habilitar debug para verificar el funcionamiento
            o.TracesSampleRate = sentrySettings.TracesSampleRate; // Capturar el 100% de las transacciones
            o.AttachStacktrace =  true; // Adjuntar trazas de pila a los errores
            o.Release = sentrySettings.Release; // Versión de la aplicación
            o.Environment = sentrySettings.Environment; // Ambiente actual
        });
    }
}