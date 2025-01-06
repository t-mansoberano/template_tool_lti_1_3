using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using CSharpFunctionalExtensions;
using gec.Application.Contracts;
using gec.Application.Contracts.Infrastructure.Canvas;
using gec.Application.Contracts.Infrastructure.Canvas.Models;
using gec.Application.Contracts.Server;
using gec.Infrastructure.Common;

namespace gec.Infrastructure.Canvas;

public class CanvasApiClient : ICanvasApiClient
{
    private readonly HttpClient _httpClient;
    private readonly ISessionStorageService _sessionStorageService;

    private const int MaxRetries = 3; // Maximum retries for rate-limited requests

    public CanvasApiClient(IHttpClientFactory httpClientFactory, ISessionStorageService sessionStorageService)
    {
        _httpClient = httpClientFactory.CreateClient("CanvasClient");
        _sessionStorageService = sessionStorageService;
    }

    public async Task<Result<T>> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default)
    {
        return await SendRequestAsync<T>(HttpMethod.Get, endpoint, null, cancellationToken);
    }

    public async Task<Result<T>> PostAsync<T>(string endpoint, object body,
        CancellationToken cancellationToken = default)
    {
        return await SendRequestAsync<T>(HttpMethod.Post, endpoint, body, cancellationToken);
    }

    public async Task<Result<T>> PutAsync<T>(string endpoint, object body,
        CancellationToken cancellationToken = default)
    {
        return await SendRequestAsync<T>(HttpMethod.Put, endpoint, body, cancellationToken);
    }

    private async Task<Result<T>> SendRequestAsync<T>(HttpMethod method, string endpoint, object? body,
        CancellationToken cancellationToken)
    {
        // Retrieve Bearer Token
        var canvasAuthData = _sessionStorageService.Retrieve<CanvasAuthToken>("CanvasAuthToken");
        if (canvasAuthData.IsFailure) return Result.Failure<T>(canvasAuthData.Error);

        _httpClient.DefaultRequestHeaders.Authorization =
            new AuthenticationHeaderValue("Bearer", canvasAuthData.Value.AccessToken);

        // Retrieve remaining quota from session storage
        var quotaResult = _sessionStorageService.Retrieve<float>("X-Rate-Limit-Remaining");
        if (quotaResult.IsSuccess && quotaResult.Value <= 700)
        {
            var delay = CalculateDynamicSleep(quotaResult.Value);
            await Task.Delay(delay, cancellationToken);
        }

        // Initialize request
        var request = new HttpRequestMessage(method, endpoint);
        if (body != null)
        {
            var jsonBody = JsonSerializer.Serialize(body);
            request.Content = new StringContent(jsonBody, Encoding.UTF8, "application/json");
        }

        // Retry Logic
        HttpResponseMessage? response = null;
        for (int attempt = 1; attempt <= MaxRetries; attempt++)
        {
            response = await _httpClient.SendAsync(request, cancellationToken);

            if (response.IsSuccessStatusCode)
                break;

            // Handle rate limit (403 with "Rate Limit Exceeded")
            if (response.StatusCode == HttpStatusCode.Forbidden &&
                response.ReasonPhrase == "Rate Limit Exceeded")
            {
                await HandleRateLimitAsync(response, attempt, cancellationToken);
                continue;
            }

            // Throw exception for non-retryable errors
            if (attempt == MaxRetries || !IsRetryableStatus(response.StatusCode))
                response.EnsureSuccessStatusCode();
        }

        if (response == null)
            return Result.Failure<T>("No hay respuesta del servidor.");

        // Check and store new quota
        if (response.Headers.TryGetValues("X-Rate-Limit-Remaining", out var values))
        {
            if (float.TryParse(values.FirstOrDefault(), out var newQuota))
            {
                _sessionStorageService.Store("X-Rate-Limit-Remaining", newQuota);
            }
        }

        // Deserialize response
        var content = await response.Content.ReadAsStringAsync(cancellationToken);
        var result = JsonSerializer.Deserialize<T>(content, new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = new SnakeCaseNamingPolicy()
        });
        
        if (result == null) 
            return Result.Failure<T>("No se pudo deserializar el resultado.");
        
        return Result.Success(result);
    }

    private async Task HandleRateLimitAsync(HttpResponseMessage response, int attempt,
        CancellationToken cancellationToken)
    {
        const int baseDelayMilliseconds = 500;
        var delay = baseDelayMilliseconds * Math.Pow(2, attempt); // Exponential backoff

        if (response.Headers.Contains("X-Rate-Limit-Reset-After"))
        {
            if (double.TryParse(response.Headers.GetValues("X-Rate-Limit-Reset-After").FirstOrDefault(),
                    out var resetAfter))
            {
                delay = Math.Max(delay, resetAfter * 1000); // Convert seconds to milliseconds
            }
        }

        Console.WriteLine($"Se ha excedido el límite de velocidad. Se volverá a intentar después de {delay} ms...");
        await Task.Delay((int)delay, cancellationToken);
    }

    private bool IsRetryableStatus(HttpStatusCode statusCode)
    {
        // Retry for 403 with "Rate Limit Exceeded" or 5xx server errors
        return statusCode == HttpStatusCode.Forbidden || (int)statusCode >= 500;
    }

    private int CalculateDynamicSleep(float remainingQuota)
    {
        if (remainingQuota <= 0)
        {
            return 15000; // Hard pause for no quota remaining
        }

        // Scale the delay dynamically based on remaining quota
        return (int)((1 - (remainingQuota / 700)) * 1000 * 10);
    }
}