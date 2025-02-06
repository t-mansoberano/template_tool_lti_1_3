using gec.Application.Common;
using gec.Application.Contracts.Infrastructure.Canvas.Enrollments.Models;
using gec.Application.Contracts.Infrastructure.Canvas.Submissions.Models;
using gec.Application.Features.Instructors.Evaluations.Dto;
using gec.Application.Features.Instructors.Evaluations.Mappers;
using Microsoft.Extensions.DependencyInjection;

namespace gec.Application;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));
        
        services.AddScoped<IMapper<Enrollment, Student>, CanvasEnrolledStudentMapper>();
        services.AddScoped<IMapper<Submission, StudentEvidences>, CanvasSubmissionStudentMapper>();

        return services;
    }
}