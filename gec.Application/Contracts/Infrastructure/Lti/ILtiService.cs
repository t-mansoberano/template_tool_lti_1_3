using CSharpFunctionalExtensions;
using gec.Application.Contracts.Infrastructure.Lti.Models;

namespace gec.Application.Contracts.Infrastructure.Lti;

public interface ILtiService
{
    Result<string> BuildAuthorizationUrl(LoginInitiationResponse form);
    Task<Result<LtiContext>> HandleRedirectAsync(Dictionary<string, string> form);
    Result<string> GetJwks();
}