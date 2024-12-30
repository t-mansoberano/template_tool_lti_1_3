using CSharpFunctionalExtensions;

namespace gec.Application.Contracts.Infrastructure.Lti.Models;

public class AuthenticationResponse
{
    public AuthenticationResponse(Dictionary<string, string> form)
    {
        Utf8 = form.GetValueOrDefault("utf8", "");
        AuthenticityToken = form.GetValueOrDefault("authenticity_token", "");
        IdToken = form.GetValueOrDefault("id_token", "");
        State = form.GetValueOrDefault("state", "");
        Error = form.GetValueOrDefault("error", "");
        ErrorDescription = form.GetValueOrDefault("error_description", "");
        LtiStorageTarget = form.GetValueOrDefault("lti_storage_target", "");
    }
    
    public string Utf8 { get; set; }
    public string AuthenticityToken { get; set; }
    public string IdToken { get; set; }
    public string State { get; set; }
    public string Error { get; set; }
    public string ErrorDescription { get; set; }
    public string LtiStorageTarget { get; set; }

    public Result<string> Validate()
    {
        var missingAttributes = new List<string>();
        
        if (string.IsNullOrWhiteSpace(IdToken)) missingAttributes.Add(nameof(IdToken));

        if (missingAttributes.Any())
        {
            var missingAttributesMessage = $"Faltan parámetros requeridos para el OIDC Launch. Los siguientes atributos están vacíos o no tienen un valor válido: {string.Join(", ", missingAttributes)}";
            return Result.Failure<string>(missingAttributesMessage);
        }

        if (!string.IsNullOrEmpty(Error))
        {
            return Result.Failure<string>(ErrorDescription);
        }

        return Result.Success(string.Empty);
    }
}