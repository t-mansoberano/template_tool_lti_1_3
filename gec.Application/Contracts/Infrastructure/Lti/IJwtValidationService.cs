using System.Security.Claims;
using CSharpFunctionalExtensions;

namespace gec.Application.Contracts.Infrastructure.Lti;

public interface IJwtValidationService
{
    Task<Result<ClaimsPrincipal>> ValidateTokenAsync(string token);
}