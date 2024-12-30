using gec.Application.Contracts.Infrastructure.Canvas;
using gec.Application.Contracts.Infrastructure.Canvas.Enrollments;
using gec.Application.Contracts.Infrastructure.Canvas.Models;
using gec.Application.Contracts.Server;
using gec.Server.Common;
using Microsoft.AspNetCore.Mvc;

namespace gec.Server.Features.Enrollments;

[ApiController]
[Route("api/canvas/courses/{courseId}/studentsss")]
public class EnrollmentsController : BaseController
{
    private readonly IEnrollmentsService _enrollmentsService;
    private readonly ICanvasOAuthService _canvasOAuthService;
    private readonly ISessionStorageService _sessionStorageService;

    public EnrollmentsController(IEnrollmentsService enrollmentsService, ICanvasOAuthService canvasOAuthService, ISessionStorageService sessionStorageService)
    {
        _enrollmentsService = enrollmentsService;
        _canvasOAuthService = canvasOAuthService;
        _sessionStorageService = sessionStorageService;
    }

    [HttpGet]
    public async Task<IActionResult> GetStudentsByCourseAsync()
    {
        var canvasAuthToken = _sessionStorageService.Retrieve<CanvasAuthToken>("CanvasAuthToken");
        if (canvasAuthToken.IsFailure)
            return Redirect(_canvasOAuthService.BuildAuthorizationUrl());
        
        canvasAuthToken = await _canvasOAuthService.GetTokenAsync(canvasAuthToken.Value);
        if (canvasAuthToken.IsFailure) return Redirect(_canvasOAuthService.BuildAuthorizationUrl());
        
        var students = await _enrollmentsService.GetStudentsByCourseAsync();
        if (students.IsFailure) 
            return Error(students.Error);

        return Ok(students);
    }
}