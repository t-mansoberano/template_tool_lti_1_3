using FluentValidation.Results;

namespace gec.Application.Common;

public static class ValidationResultExtension
{
    public static string ErrorMessages(this ValidationResult validationResult)
    {
        return string.Join(",", validationResult.Errors.Select(s => s.ErrorMessage));
    }
}