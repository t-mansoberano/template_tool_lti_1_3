namespace gec.Server.Startup;

public static class ConfigurationSources
{
    /// <summary>
    ///     Agrega las fuentes de configuración necesarias a IConfigurationBuilder.
    /// </summary>
    public static IConfigurationBuilder AddConfigurationSources(this IConfigurationBuilder configurationBuilder,
        IHostEnvironment environment)
    {
        return configurationBuilder
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", false, true)
            .AddJsonFile($"appsettings.{environment.EnvironmentName}.json", true, true)
            .AddJsonFile("appsettings.Secrets.json", true, true) // Cargar secretos si existe
            .AddEnvironmentVariables(); // Priorizar variables de entorno
    }
}