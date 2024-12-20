using System.Security.Claims;
using CSharpFunctionalExtensions;

namespace gec.Infrastructure.Lti;

public interface IJwtValidationService
{
    Task<Result<ClaimsPrincipal>> ValidateTokenAsync(string token);
}