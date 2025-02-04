using FluentValidation;

namespace gec.Application.Features.Instructors.Evaluations.Queries.GetTestForCanvasApi;

public class GetTestForCanvasApiQueryValidator : AbstractValidator<GetTestForCanvasApiQuery>
{
    public GetTestForCanvasApiQueryValidator()
    {
        RuleFor(x => x.CourseId)
            .NotEmpty()
            .WithMessage("CourseId es requerido");

        RuleFor(x => x)
            .MustAsync(ExistsOtherValidationAsync)
            .WithMessage("Otro error async");

        RuleFor(x => x)
            .Must(ExistsOtherValidation)
            .WithMessage("Otro error");
    }

    private async Task<bool> ExistsOtherValidationAsync(GetTestForCanvasApiQuery query,
        CancellationToken cancellationToken)
    {
        return await Task.FromResult(true);
    }

    private bool ExistsOtherValidation(GetTestForCanvasApiQuery query)
    {
        return true;
    }
}