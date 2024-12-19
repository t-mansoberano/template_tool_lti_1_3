using System.Runtime.InteropServices.JavaScript;
using System.Text.Json;
using gec.Infrastructure.Canvas;
using gec.Infrastructure.Lti;
using gec.Infrastructure.Lti.Models;
using gec.Server.Common;
using gec.Server.Controllers.Lti;
using Microsoft.AspNetCore.Mvc;

namespace gec.Server.Controllers.Canvas.Courses;

[ApiController]
[Route("api/canvas/courses/{courseId}/students")]
public class StudentsController : BaseController
{
    private readonly ICanvasService _canvasService;
    private readonly ILtiService _ltiService;

    public StudentsController(ICanvasService canvasService, ILtiService ltiService)
    {
        _canvasService = canvasService;
        _ltiService = ltiService;
    }

    [HttpGet]
    public async Task<IActionResult> GetStudents()
    {
        // Obtener el contexto del recurso desde la sesión
        var resourceContextJson = HttpContext.Session.GetString("resourceContext");
        var tokenResponseJson = HttpContext.Session.GetString("tokenResponse");
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

        try
        {
            // Obtener la lista de estudiantes usando el token y el ID del curso
            var students = await _canvasService.GetStudentsAsync("", resourceContext.Course.Id);
            return Ok(students);
        }
        catch (Exception ex)
        {
            return StatusCode(500, $"Error al obtener la lista de estudiantes: {ex.Message}");
        }
    }
}
