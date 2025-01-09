using CSharpFunctionalExtensions;

namespace gec.Application.Contracts.Infrastructure.Canvas.OAuth.Models;

public class OAuthQuery
{
    // Constructor para inicializar desde el query string
    public OAuthQuery(Dictionary<string, string> query)
    {
        Code = query.GetValueOrDefault("code", "");
        State = query.GetValueOrDefault("state", "");
        Error = query.GetValueOrDefault("error", "");
    }

    public string Code { get; set; }
    public string State { get; set; }
    public string Error { get; set; }

    // Método de validación
    public Result Validate()
    {
        if (!string.IsNullOrEmpty(Error)) return Result.Failure($"Error recibido en la respuesta: {Error}");

        if (string.IsNullOrEmpty(Code)) return Result.Failure("No se recibió el código de autorización.");

        return Result.Success();
    }
}