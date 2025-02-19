using System.Text.Json;
using CSharpFunctionalExtensions;
using gec.Application.Contracts.Server.Fake;

namespace gec.Server.Common;

public class FakeDataService : IFakeDataService
{
    private readonly IWebHostEnvironment _environment;

    public FakeDataService(IWebHostEnvironment environment)
    {
        _environment = environment;
    }

    public Result<T> GetFakeData<T>(string relativeJsonPath)
    {
        // Combinar la ruta raíz con la ruta relativa del archivo JSON
        var fullPath = Path.Combine(_environment.ContentRootPath, relativeJsonPath);

        if (!File.Exists(fullPath))
        {
            return Result.Failure<T>($"No se encontró el archivo JSON en: {fullPath}");
        }

        try
        {
            var json = File.ReadAllText(fullPath);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            var data = JsonSerializer.Deserialize<T>(json, options);

            if (data == null)
            {
                return Result.Failure<T>("Error al deserializar el contenido del JSON.");
            }

            return data;
        }
        catch (Exception ex)
        {
            return Result.Failure<T>($"Ocurrió un error al leer el archivo JSON: {ex.Message}");
        }
    }
}