using CSharpFunctionalExtensions;

namespace gec.Application.Contracts.Infrastructure.Canvas.Api;

public interface ICanvasApiClient
{
    Task<Result<T>> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default);
    Task<Result<T>> PostAsync<T>(string endpoint, object body, CancellationToken cancellationToken = default);
    Task<Result<T>> PutAsync<T>(string endpoint, object body, CancellationToken cancellationToken = default);
}