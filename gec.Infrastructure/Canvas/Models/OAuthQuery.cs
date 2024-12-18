﻿namespace gec.Infrastructure.Canvas.Models;

using CSharpFunctionalExtensions;

public class OAuthQuery
{
    public string Code { get; set; }
    public string State { get; set; }
    public string Error { get; set; }

    // Constructor para inicializar desde el query string
    public OAuthQuery(Dictionary<string, string> query)
    {
        Code = query.GetValueOrDefault("code", null);
        State = query.GetValueOrDefault("state", null);
        Error = query.GetValueOrDefault("error", null);
    }

    // Método de validación
    public Result Validate()
    {
        if (!string.IsNullOrEmpty(Error))
        {
            return Result.Failure($"Error recibido en la respuesta: {Error}");
        }

        if (string.IsNullOrEmpty(Code))
        {
            return Result.Failure("No se recibió el código de autorización.");
        }

        return Result.Success();
    }
}
