using gec.Application;
using gec.Infrastructure;
using Serilog;

namespace gec.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            ConfigureLogging();

            try
            {
                var builder = WebApplication.CreateBuilder(args);
                ConfigureServices(builder);
            
                var app = builder.Build();
                ConfigurePipeline(app);

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

        private static void ConfigureServices(WebApplicationBuilder builder)
        {
            builder.Services.AddApplicationServices();
            builder.Services.AddServerServices();
            builder.Services.AddInfrastructureServices(builder.Configuration);

            builder.Services.AddDistributedMemoryCache();
            builder.Services.AddSession(options =>
            {
                options.Cookie.Domain = "manuelsoberano.ngrok.dev";
                options.IdleTimeout = TimeSpan.FromMinutes(480); // Tiempo de expiración de la sesión
                options.Cookie.HttpOnly = true; // Previene acceso desde JavaScript
                options.Cookie.IsEssential = true; // Necesario para cumplimiento de GDPR
                options.Cookie.SecurePolicy = CookieSecurePolicy.Always; // Solo envía cookies en HTTPS
                options.Cookie.SameSite = SameSiteMode.None; // Permite cookies en solicitudes cruzadas
            });

            builder.Services.AddHttpContextAccessor();
            
            builder.Services.AddCors(options =>
            {
                options.AddDefaultPolicy(policy =>
                    policy.WithOrigins("https://localhost:4200",
                            "https://localhost:7051",
                            "http://localhost:5156",
                            "https://manuelsoberano.ngrok.dev")
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials());
            });
            builder.Services.AddHttpClient();
            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();
        }

        private static void ConfigurePipeline(WebApplication app)
        {
            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseCors();
            app.UseSession();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.MapFallbackToFile("/index.html");
        }

        private static void ConfigureLogging()
        {
            // Configurar Serilog
            Log.Logger = new LoggerConfiguration()
                .WriteTo.Console() // Logs en consola
                .WriteTo.File(@"C:\Log\GestorEvaluacionesCompetencia-.log", rollingInterval: RollingInterval.Day) // Logs en archivos rotados
                .WriteTo.Sentry(o =>
                {
                    o.Dsn = "https://70801557168fc87f254a77ff783afb6c@o4508458878500864.ingest.us.sentry.io/4508458976870400";
                    o.MinimumBreadcrumbLevel = Serilog.Events.LogEventLevel.Debug;
                    o.MinimumEventLevel = Serilog.Events.LogEventLevel.Debug;
                    o.AttachStacktrace = true;
                    o.Release = "gec@0.0.1";
                    o.Environment = "Development";
                    // When configuring for the first time, to see what the SDK is doing:
                    o.Debug = true;
                    // Set TracesSampleRate to 1.0 to capture 100%
                    // of transactions for tracing.
                    // We recommend adjusting this value in production
                    o.TracesSampleRate = 1.0;                    
                })
                .CreateLogger();            
        }
    }
}