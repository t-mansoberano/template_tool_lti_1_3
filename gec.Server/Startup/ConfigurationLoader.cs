namespace gec.Server.Startup;

public static class ConfigurationSources
{
    /// <summary>
    /// Agrega las fuentes de configuración necesarias a IConfigurationBuilder.
    /// </summary>
    public static IConfigurationBuilder AddConfigurationSources(this IConfigurationBuilder configurationBuilder, IHostEnvironment environment)
    {
        return configurationBuilder
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddJsonFile("appsettings.Secrets.json", optional: true, reloadOnChange: true) // Cargar secretos si existe
            .AddEnvironmentVariables(); // Priorizar variables de entorno
    }
}