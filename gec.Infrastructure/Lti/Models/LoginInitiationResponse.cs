using CSharpFunctionalExtensions;

namespace gec.Infrastructure.Lti.Models;

public class LoginInitiationResponse
{
    public LoginInitiationResponse(Dictionary<string, string> form)
    {
        Iss = form.GetValueOrDefault("iss", "");
        LoginHint = form.GetValueOrDefault("login_hint", "");
        LtiMessageHint = form.GetValueOrDefault("lti_message_hint", "");
        ClientId = form.GetValueOrDefault("client_id", "");
        TargetLinkUri = form.GetValueOrDefault("target_link_uri", "");
    }

    public string Iss { get; set; }
    public string LoginHint { get; set; }
    public string LtiMessageHint { get; set; }
    public string ClientId { get; set; }
    public string TargetLinkUri { get; set; }

    public Result<string> Validate()
    {
        var missingAttributes = new List<string>();

        if (string.IsNullOrWhiteSpace(Iss)) missingAttributes.Add(nameof(Iss));
        if (string.IsNullOrWhiteSpace(LoginHint)) missingAttributes.Add(nameof(LoginHint));
        if (string.IsNullOrWhiteSpace(LtiMessageHint)) missingAttributes.Add(nameof(LtiMessageHint));
        if (string.IsNullOrWhiteSpace(ClientId)) missingAttributes.Add(nameof(ClientId));
        if (string.IsNullOrWhiteSpace(TargetLinkUri)) missingAttributes.Add(nameof(TargetLinkUri));

        if (missingAttributes.Any())
        {
            var missingAttributesMessage = $"Faltan parámetros requeridos para el OIDC Launch. Los siguientes atributos están vacíos o no tienen un valor válido: {string.Join(", ", missingAttributes)}";
            return Result.Failure<string>(missingAttributesMessage);
        }

        return Result.Success(string.Empty);
    }
}
