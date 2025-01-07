using System.Text.Json;
using CSharpFunctionalExtensions;
using gec.Application.Contracts.Server;

namespace gec.Server.Common;

public class SessionStorageService : ISessionStorageService
{
    private readonly ISession _session;

    public SessionStorageService(IHttpContextAccessor httpContextAccessor)
    {
        _session = httpContextAccessor.HttpContext?.Session
                   ?? throw new InvalidOperationException("El HttpContext o la sesión no está disponible.");
    }

    public Result Store<T>(string key, T value)
    {
        if (string.IsNullOrEmpty(key))
            return Result.Failure("La clave no puede ser nula o vacía.");

        if (value == null)
            return Result.Failure("El valor no puede ser nulo.");

        var json = JsonSerializer.Serialize(value);
        if (json.Length > 4096)
            return Result.Failure("El tamaño del objeto excede el límite permitido.");

        _session.SetString(key, json);

        return Result.Success();
    }

    public Result<T> Retrieve<T>(string key)
    {
        if (string.IsNullOrEmpty(key))
            return Result.Failure<T>("La clave no puede ser nula o vacía.");

        var json = _session.GetString(key);
        if (string.IsNullOrEmpty(json))
            return Result.Failure<T>($"No se encontró ningún valor asociado a la clave: {key}.");

        try
        {
            var deserializedValue = JsonSerializer.Deserialize<T>(json);
            if (deserializedValue == null)
                return Result.Failure<T>($"No se pudo deserializar el valor para la clave: {key}.");

            return Result.Success(deserializedValue);
        }
        catch (Exception e)
        {
            return Result.Failure<T>($"Error al deserializar el valor almacenado: {e.Message}");
        }
    }

    public Result Remove(string key)
    {
        if (string.IsNullOrEmpty(key))
            return Result.Failure("La clave no puede ser nula o vacía.");

        _session.Remove(key);

        return Result.Success();
    }
}