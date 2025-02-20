using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security;
using ITfoxtec.Identity.Saml2;
using ITfoxtec.Identity.Saml2.MvcCore.Configuration;
using ITfoxtec.Identity.Saml2.Schemas.Metadata;
using ITfoxtec.Identity.Saml2.Util;

namespace gec.Server.Startup;

    public static class FederationExtensions
    {
        public static IServiceCollection AddFederationServices(this IServiceCollection services, IConfiguration configuration)
        {
            // Registrar la configuración de Saml2 desde el appsettings.json
            services.Configure<Saml2Configuration>(configuration.GetSection("Saml2"));
            
            // Configurar de forma centralizada Saml2Configuration
            services.Configure<Saml2Configuration>(saml2Configuration =>
            {
                try
                {
                    saml2Configuration.Issuer = configuration["Saml2:Issuer"];
                    saml2Configuration.SingleSignOnDestination = new Uri(configuration["Saml2:SingleSignOnDestination"]);
                    saml2Configuration.SingleLogoutDestination = new Uri(configuration["Saml2:SingleLogoutDestination"]);
                    saml2Configuration.SignatureAlgorithm = configuration["Saml2:SignatureAlgorithm"];
                    saml2Configuration.SignAuthnRequest = Convert.ToBoolean(configuration["Saml2:SignAuthnRequest"]);
                    saml2Configuration.SigningCertificate = CertificateUtil.Load(
                        configuration["Saml2:SigningCertificateFile"],
                        configuration["Saml2:SigningCertificatePassword"],
                        X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet | X509KeyStorageFlags.PersistKeySet);
                    saml2Configuration.CertificateValidationMode = (X509CertificateValidationMode)Enum.Parse(
                        typeof(X509CertificateValidationMode), configuration["Saml2:CertificateValidationMode"]);
                    saml2Configuration.RevocationMode = (X509RevocationMode)Enum.Parse(
                        typeof(X509RevocationMode), configuration["Saml2:RevocationMode"]);
                    saml2Configuration.AllowedAudienceUris.Add(saml2Configuration.Issuer);

                    // Cargar metadata del IdP para actualizar endpoints y certificados de validación
                    var entityDescriptor = new EntityDescriptor();
                    entityDescriptor.ReadIdPSsoDescriptorFromUrl(new Uri(configuration["Saml2:IdPMetadata"]));
                    if (entityDescriptor.IdPSsoDescriptor != null)
                    {
                        saml2Configuration.SingleSignOnDestination = entityDescriptor.IdPSsoDescriptor
                            .SingleSignOnServices.First().Location;
                        saml2Configuration.SingleLogoutDestination = entityDescriptor.IdPSsoDescriptor
                            .SingleLogoutServices.First().Location;
                        saml2Configuration.SignatureValidationCertificates.AddRange(
                            entityDescriptor.IdPSsoDescriptor.SigningCertificates);
                    }
                    else
                    {
                        throw new Exception("No se pudo cargar el IdPSsoDescriptor del metadata");
                    }
                }
                catch (Exception ex)
                {
                    // Aquí se podría loggear el error de forma centralizada
                    throw new Exception("Error configurando Saml2", ex);
                }
            });

            // Registrar el servicio Saml2 con la opción de expiración deslizante
            services.AddSaml2(slidingExpiration: true);

            return services;
        }

        public static IApplicationBuilder UseFederationMiddleware(this IApplicationBuilder app)
        {
#if !DEBUG
            // Solo aplicar la configuración de federación en ambientes de publicación
            app.UseSaml2();

            // Ejemplo de mapeo de rutas para forzar autenticación en ciertos endpoints (por ejemplo, API)
            app.MapWhen(context =>
            {
                return !context.User.Identity.IsAuthenticated &&
                       context.Request.Path.Value.StartsWith("/api/", StringComparison.OrdinalIgnoreCase);
            },
            config =>
            {
                config.Run(async context =>
                {
                    context.Response.StatusCode = (int)System.Net.HttpStatusCode.Unauthorized;
                    await context.Response.CompleteAsync();
                });
            });
#endif
            return app;
        }
    }