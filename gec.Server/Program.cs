using gec.Server.Startup;
using ITfoxtec.Identity.Saml2.MvcCore.Configuration;
using ITfoxtec.Identity.Saml2.Schemas.Metadata;
using ITfoxtec.Identity.Saml2.Util;
using ITfoxtec.Identity.Saml2;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel.Security;

namespace gec.Server;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Configurar fuentes de configuración
        builder.Configuration.AddConfigurationSources(builder.Environment);

        // Configurar servicios de servidor
        builder.Services.AddServerServices(builder.Configuration);
        builder.Services.AddLayerServices();

        builder.Services.AddCustomServices(builder.Configuration);

        // Configurar un logger
        LoggingConfiguration.ConfigureSerilog(builder.Configuration);

        // Configurar Sentry
        builder.WebHost.ConfigureSentry(builder.Configuration);

        #region Federacion
        builder.Services.Configure<Saml2Configuration>(builder.Configuration.GetSection("Saml2"));
        builder.Services.Configure<Saml2Configuration>(saml2Configuration =>
        {
            try
            {

                saml2Configuration.Issuer = builder.Configuration["Saml2:Issuer"];
                saml2Configuration.SingleSignOnDestination = new Uri(builder.Configuration["Saml2:SingleSignOnDestination"]);
                saml2Configuration.SingleLogoutDestination = new Uri(builder.Configuration["Saml2:SingleLogoutDestination"]);
                saml2Configuration.SignatureAlgorithm = builder.Configuration["Saml2:SignatureAlgorithm"];
                saml2Configuration.SignAuthnRequest = Convert.ToBoolean(builder.Configuration["Saml2:SignAuthnRequest"]);
                saml2Configuration.SigningCertificate = CertificateUtil.Load(builder.Configuration["Saml2:SigningCertificateFile"], builder.Configuration["Saml2:SigningCertificatePassword"],
                                                                                                                            X509KeyStorageFlags.Exportable | X509KeyStorageFlags.MachineKeySet |
                                                                                                                            X509KeyStorageFlags.PersistKeySet);
                saml2Configuration.CertificateValidationMode = (X509CertificateValidationMode)Enum.Parse(typeof(X509CertificateValidationMode), builder.Configuration["Saml2:CertificateValidationMode"]);
                saml2Configuration.RevocationMode = (X509RevocationMode)Enum.Parse(typeof(X509RevocationMode), builder.Configuration["Saml2:RevocationMode"]);
                saml2Configuration.AllowedAudienceUris.Add(saml2Configuration.Issuer);
                var entityDescriptor = new EntityDescriptor();
                entityDescriptor.ReadIdPSsoDescriptorFromUrl(new Uri(builder.Configuration["Saml2:IdPMetadata"]));
                if (entityDescriptor.IdPSsoDescriptor != null)
                {
                    saml2Configuration.SingleSignOnDestination = entityDescriptor.IdPSsoDescriptor.SingleSignOnServices.First().Location;
                    saml2Configuration.SingleLogoutDestination = entityDescriptor.IdPSsoDescriptor.SingleLogoutServices.First().Location;
                    saml2Configuration.SignatureValidationCertificates.AddRange(entityDescriptor.IdPSsoDescriptor.SigningCertificates);
                }
                else
                    throw new Exception("No se cargó el IdPSsoDescriptor del metadata");
            }
            catch (Exception)
            {
                throw;
            }
        });

        builder.Services.AddSaml2(slidingExpiration: true);
        #endregion

        var app = builder.Build();

#if !DEBUG // Con esta condicíón, solo se va a ejecutar este código cuando se ejecute en modo Release. Esto permite que se ejecute la configuración de la federación solo en los ambientes de publicación.
        app.UseSaml2();
        app.MapWhen(
            context =>
            {
                return !context.User.Identity.IsAuthenticated && context.Request.Path.Value.StartsWith("/api/", StringComparison.OrdinalIgnoreCase);
            },
            config =>
            {
                config.Run(async context =>
                {
                    context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    await Task.FromResult(string.Empty);
                });
            }
        );
#endif



        // Configurar middleware
        app.UseCustomMiddleware();

        app.Run();
    }
}