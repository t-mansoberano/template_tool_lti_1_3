using CSharpFunctionalExtensions;
using gec.Infrastructure.Lti.Models;

namespace gec.Infrastructure.Lti;

public interface ILtiService
{
    Result<string> BuildAuthorizationUrl(LoginInitiationResponse form);
    Task<Result<ResourceContext>> HandleRedirectAsync(Dictionary<string, string> form);
    Result<string> GetJwks();
}