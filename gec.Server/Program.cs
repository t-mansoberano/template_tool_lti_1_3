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
            builder.Services.AddInfrastructureServices();
            
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
    }
}