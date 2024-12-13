using CSharpFunctionalExtensions;

namespace gec.Infrastructure.Lti.Models;

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

        if (!string.IsNullOrEmpty(Error))
        {
            return Result.Failure<string>(ErrorDescription);
        }

        return Result.Success(string.Empty);
    }
}