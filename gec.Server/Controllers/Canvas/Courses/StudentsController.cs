using System.Text.Json;
using gec.Infrastructure.Canvas;
using gec.Infrastructure.Lti.Models;
using gec.Server.Common;
using Microsoft.AspNetCore.Mvc;

namespace gec.Server.Controllers.Canvas.Courses;

[ApiController]
[Route("api/canvas/courses/{courseId}/students")]
public class StudentsController : BaseController
{
    private readonly ICanvasService _canvasService;
    private readonly ICanvasOAuthService _canvasOAuthService;

    public StudentsController(ICanvasService canvasService, ICanvasOAuthService canvasOAuthService)
    {
        _canvasService = canvasService;
        _canvasOAuthService = canvasOAuthService;
    }

    [HttpGet]
    public async Task<IActionResult> GetStudents()
    {
        // Obtener el contexto del recurso desde la sesión
        var resourceContextJson = HttpContext.Session.GetString("ResourceContext");
        var tokenResponseJson = HttpContext.Session.GetString("CanvasToken");
        if (string.IsNullOrEmpty(resourceContextJson))
        {
            return Error("El contexto del recurso no está disponible en la sesión.");
        }
        if (string.IsNullOrEmpty(tokenResponseJson))
        {
            return Error("El token response no está disponible en la sesión.");
        }
        
        // Deserializar el contexto del recurso
        var resourceContext = JsonSerializer.Deserialize<ResourceContext>(resourceContextJson);
        var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(tokenResponseJson);

        try
        {
            // Actualizar el token del servicio de Canvas
            var token = await _canvasOAuthService.GetTokenAsync(tokenResponse);
            if (token.IsFailure) return Redirect("/api/lti/oauth/authorize");

            // Obtener la lista de estudiantes usando el token y el ID del curso
            var students = await _canvasService.GetStudentsByCourseAsync(token.Value.AccessToken, resourceContext.Course.Id);
            return Ok(students);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al obtener la lista de estudiantes: {ex.Message}");
        }
    }
}
