using CSharpFunctionalExtensions;
using gec.Application.Contracts.Infrastructure.Federation.Models;

namespace gec.Application.Contracts.Infrastructure.Federation;

public interface IFederationService
{
    Task<Result<FederetionContext>> HandleAuthAsync();
}