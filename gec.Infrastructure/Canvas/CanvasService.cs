using System.Net.Http.Headers;
using System.Text.Json;
using gec.Infrastructure.Canvas.Models;
using gec.Infrastructure.Common;

namespace gec.Infrastructure.Canvas;

public class CanvasService : ICanvasService
{
    private readonly AppSettingsService _appSettings;
    private readonly HttpClient _httpClient;

    public CanvasService(AppSettingsService appSettings, IHttpClientFactory httpClientFactory)
    {
        _appSettings = appSettings;
        _httpClient = httpClientFactory.CreateClient("CanvasClient");
    }

    public async Task<IEnumerable<Enrollment>> GetStudentsByCourseAsync(string accessToken, string courseId)
    {
        var url = $"/api/v1/courses/{courseId}/enrollments";

        // Configurar el encabezado de autorización con el token
        _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);

        try
        {
            // Realizar la solicitud GET a la API de Canvas
            var response = await _httpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();

            // Leer el contenido de la respuesta
            var jsonResponse = await response.Content.ReadAsStringAsync();

            // Opciones de deserialización
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = new SnakeCaseNamingPolicy()
            }; 
            
            // Deserializar la respuesta JSON
            var students = JsonSerializer.Deserialize<List<Enrollment>>(jsonResponse, options);

            return students;
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Error al obtener estudiantes del curso: {ex.Message}");
        }
    }

}