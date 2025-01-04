using gec.Application.Contracts.Server.Configuration;

namespace gec.Server.Startup;

public static class SentryConfiguration
{
    public static void ConfigureSentry(this IWebHostBuilder webHostBuilder, IAppSettingsService appSettings)
    {
        webHostBuilder.UseSentry(o =>
        {
            o.Dsn = appSettings.Sentry.Dsn;
            o.Debug = appSettings.Sentry.Debug; // Habilitar debug para verificar el funcionamiento
            o.TracesSampleRate = appSettings.Sentry.TracesSampleRate; // Capturar el 100% de las transacciones
            o.AttachStacktrace =  true; // Adjuntar trazas de pila a los errores
            o.Release = appSettings.Sentry.Release; // Versión de la aplicación
            o.Environment = appSettings.Sentry.Environment; // Ambiente actual
        });
    }
}