using gec.Server.Startup;
using Serilog;

namespace gec.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            LoggingConfiguration.ConfigureSerilog();

            try
            {
                var builder = WebApplication.CreateBuilder(args);

                // Configurar servicios de servidor
                builder.Services.AddServerServices();
                builder.Services.AddCustomServices(builder.Configuration);
                
                // Configurar Sentry
                builder.WebHost.ConfigureSentry();
                
                var app = builder.Build();
                
                // Configurar middleware
                app.UseCustomMiddleware();

                app.Run();
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "La aplicación no pudo iniciarse correctamente.");
                throw;
            }
            finally
            {
                Log.CloseAndFlush(); // Asegurar que los logs pendientes se envíen antes de cerrar
            }
        }
    }
}