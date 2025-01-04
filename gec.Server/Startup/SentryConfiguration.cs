namespace gec.Server.Startup;

public static class SentryConfiguration
{
    public static void ConfigureSentry(this IWebHostBuilder webHostBuilder)
    {
        webHostBuilder.UseSentry(o =>
        {
            o.Dsn = "https://70801557168fc87f254a77ff783afb6c@o4508458878500864.ingest.us.sentry.io/4508458976870400";
            o.Debug = true; // Habilitar debug para verificar el funcionamiento
            o.TracesSampleRate = 1.0; // Capturar el 100% de las transacciones
            o.AttachStacktrace = true; // Adjuntar trazas de pila a los errores
            o.Release = "gec@0.0.1"; // Versión de la aplicación
            o.Environment = "Development"; // Ambiente actual
        });
    }
}