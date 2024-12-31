using gec.Application.Contracts.Infrastructure.Canvas;
using gec.Application.Contracts.Infrastructure.Canvas.Models;
using gec.Application.Contracts.Server;
using gec.Application.Features.Teachers.Evaluations.Queries.GetTestForCanvasAPI;
using gec.Server.Common;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace gec.Server.Features.Teachers.Evaluations;


[ApiController]
[Route("api/teachers/courses/{courseId}/testapicanvas")]
public class GetTestForCanvasAPIController : BaseController
{
    private readonly IMediator _mediator;
    private readonly ICanvasOAuthService _canvasOAuthService;
    private readonly ISessionStorageService _sessionStorageService;

    public GetTestForCanvasAPIController(IMediator mediator, ICanvasOAuthService canvasOAuthService,
        ISessionStorageService sessionStorageService)
    {
        _mediator = mediator;
        _canvasOAuthService = canvasOAuthService;
        _sessionStorageService = sessionStorageService;
    }

    [HttpGet]
    public async Task<IActionResult> GetStudentsByCourseAsync([FromQuery] GetTestForCanvasAPIQuery query)
    {
        var canvasAuthToken = _sessionStorageService.Retrieve<CanvasAuthToken>("CanvasAuthToken");
        if (canvasAuthToken.IsFailure)
            return Redirect(_canvasOAuthService.BuildAuthorizationUrl());

        canvasAuthToken = await _canvasOAuthService.GetTokenAsync(canvasAuthToken.Value);
        if (canvasAuthToken.IsFailure) return Redirect(_canvasOAuthService.BuildAuthorizationUrl());

        var result = await _mediator.Send(query);
        if (result.IsFailure)
            return Error(result.Error);

        return Ok(result.Value);
    }

}