namespace gec.Infrastructure.Common;

using Microsoft.Extensions.Configuration;

public class AppSettingsService
{
    public string CanvasBaseUrl { get; }
    public string CanvasTokenEndpoint { get; }
    public string CanvasClientId { get; }
    public string CanvasClientSecret { get; }
    public string CanvasRedirectUri { get; }

    public string LtiUrlBase { get; }
    public string LtiValidIssuer { get; }
    public string LtiRedirectUri { get; }
    public string LtiClientId { get; }
    public string LtiClientSecret { get; }

    public AppSettingsService(IConfiguration configuration)
    {
        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration), "La configuración no puede ser null.");
        }

        // LtiSettings
        LtiUrlBase = configuration["LtiSettings:UrlBase"]
                     ?? throw new InvalidOperationException("LtiSettings:UrlBase no está configurado.");
        LtiValidIssuer = configuration["LtiSettings:ValidIssuer"]
                      ?? throw new InvalidOperationException("LtiSettings:ValidIssuer no está configurado.");
        LtiRedirectUri = configuration["LtiSettings:RedirectUri"]
                         ?? throw new InvalidOperationException("LtiSettings:RedirectUri no está configurado.");
        LtiClientId = configuration["LtiSettings:ClientId"]
                      ?? throw new InvalidOperationException("LtiSettings:ClientId no está configurado.");
        LtiClientSecret = configuration["LtiSettings:ClientSecret"]
                          ?? throw new InvalidOperationException("LtiSettings:ClientSecret no está configurado.");

        // Canvas
        CanvasBaseUrl = configuration["Canvas:ApiBaseUrl"]
                        ?? throw new InvalidOperationException("Canvas:ApiBaseUrl no está configurado.");
        CanvasTokenEndpoint = configuration["Canvas:TokenEndpoint"]
                              ?? throw new InvalidOperationException("Canvas:TokenEndpoint no está configurado.");
        CanvasClientId = configuration["Canvas:ClientId"]
                         ?? throw new InvalidOperationException("Canvas:ClientId no está configurado.");
        CanvasClientSecret = configuration["Canvas:ClientSecret"]
                             ?? throw new InvalidOperationException("Canvas:ClientSecret no está configurado.");
        CanvasRedirectUri = configuration["Canvas:RedirectUri"]
                            ?? throw new InvalidOperationException("Canvas:RedirectUri no está configurado.");
    }
}
