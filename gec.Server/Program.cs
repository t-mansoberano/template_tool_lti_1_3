using gec.Infrastructure;

namespace gec.Server
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            
            ConfigureServices(builder);
            
            var app = builder.Build();

            ConfigurePipeline(app);

            app.Run();
        }

        private static void ConfigureServices(WebApplicationBuilder builder)
        {
            builder.Services.AddServerServices();
            builder.Services.AddInfrastructureServices(builder.Configuration);

            builder.WebHost.UseSentry(o =>
            {
                o.Dsn = "https://70801557168fc87f254a77ff783afb6c@o4508458878500864.ingest.us.sentry.io/4508458976870400";
                // When configuring for the first time, to see what the SDK is doing:
                o.Debug = true;
                // Set TracesSampleRate to 1.0 to capture 100%
                // of transactions for tracing.
                // We recommend adjusting this value in production
                o.TracesSampleRate = 1.0;
            });

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
        
        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    // Add the following line:
                    webBuilder.UseSentry(o =>
                    {
                        o.Dsn = "https://70801557168fc87f254a77ff783afb6c@o4508458878500864.ingest.us.sentry.io/4508458976870400";
                        // When configuring for the first time, to see what the SDK is doing:
                        o.Debug = true;
                        // Set TracesSampleRate to 1.0 to capture 100%
                        // of transactions for tracing.
                        // We recommend adjusting this value in production
                        o.TracesSampleRate = 1.0;
                    });
                });
    }
}