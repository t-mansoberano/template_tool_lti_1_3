using gec.Application.Common;
using gec.Application.Contracts.Infrastructure.Canvas.Enrollments.Models;
using gec.Application.Features.Instructors.Evaluations.Dto;
using gec.Application.Features.Instructors.Evaluations.Mappers;
using Microsoft.Extensions.DependencyInjection;

namespace gec.Application;

public static class ServiceRegistration
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.GetAssemblies()));
        
        services.AddScoped<IMapper<Enrollment, StudentEvaluation>, CanvasEnrolledStudentMapper>();

        return services;
    }
}