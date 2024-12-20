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

    public async Task<IEnumerable<Student>> GetStudentsByCourseAsync(string accessToken, string courseId)
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

            // Deserializar la respuesta JSON
            var students = JsonSerializer.Deserialize<List<CanvasEnrollment>>(jsonResponse, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            // Mapear los datos al modelo Student
            var studentList = students?
                .Where(e => e.User != null)
                .Select(e => new Student
                {
                    Id = e.User.Id,
                    Name = e.User.Name,
                    Email = e.User.Email
                }).ToList();

            return studentList ?? new List<Student>();
        }
        catch (HttpRequestException ex)
        {
            throw new Exception($"Error al obtener estudiantes del curso: {ex.Message}");
        }
    }

}