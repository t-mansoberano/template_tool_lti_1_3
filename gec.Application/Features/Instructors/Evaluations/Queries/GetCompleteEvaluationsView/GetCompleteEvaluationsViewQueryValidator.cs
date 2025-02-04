using FluentValidation;

namespace gec.Application.Features.Instructors.Evaluations.Queries.GetCompleteEvaluationsView;

public class GetCompleteEvaluationsViewQueryValidator : AbstractValidator<GetCompleteEvaluationsViewQuery>
{
    public GetCompleteEvaluationsViewQueryValidator()
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

    private async Task<bool> ExistsOtherValidationAsync(GetCompleteEvaluationsViewQuery query,
        CancellationToken cancellationToken)
    {
        return await Task.FromResult(true);
    }

    private bool ExistsOtherValidation(GetCompleteEvaluationsViewQuery query)
    {
        return true;
    }
}