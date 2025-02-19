using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using gec.Application.Contracts.Infrastructure.Canvas.Api;
using gec.Application.Contracts.Infrastructure.Canvas.OAuth.Models;
using gec.Application.Contracts.Server.Configuration;
using gec.Application.Contracts.Server.Session;
using gec.Infrastructure.Common;

namespace gec.Infrastructure.Canvas.Api;

public class CanvasApiClient : ICanvasApiClient
{
    private const int MaxRetries = 3; // Maximum retries for rate-limited requests
    private readonly HttpClient _httpClient;
    private readonly ISessionStorageService _sessionStorageService;
    private readonly IAppSettingsService _appSettings;

    public CanvasApiClient(IHttpClientFactory httpClientFactory, ISessionStorageService sessionStorageService, IAppSettingsService appSettings)
    {
        _httpClient = httpClientFactory.CreateClient("CanvasClient");
        _sessionStorageService = sessionStorageService;
        _appSettings = appSettings;
    }

    public async Task<Result<List<T>>> GetPaginatedAsync<T>(string endpoint, CancellationToken cancellationToken = default)
    {
        // Agregar `per_page` dinámicamente si no está en la URL
        endpoint = EnsurePerPageInUrl(endpoint, _appSettings.Canvas.PerPage);

        List<T> allResults = new List<T>();
        string? nextUrl = endpoint;

        while (!string.IsNullOrEmpty(nextUrl))
        {
            var result = await GetAsync<List<T>>(nextUrl, cancellationToken);

            if (result.IsFailure)
                return Result.Failure<List<T>>(result.Error);

            if (result.Value != null)
                allResults.AddRange(result.Value);

            // Obtener la URL de la siguiente página desde las cabeceras
            nextUrl = await GetNextPageUrlAsync(nextUrl, cancellationToken);
        }

        return Result.Success(allResults);
    }

    private async Task<string?> GetNextPageUrlAsync(string currentUrl, CancellationToken cancellationToken)
    {
        var request = new HttpRequestMessage(HttpMethod.Head, currentUrl); // Solo pedimos cabeceras
        var response = await _httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
            return null;

        if (response.Headers.TryGetValues("Link", out var linkHeaders))
        {
            string linkHeader = string.Join(",", linkHeaders);
            var match = Regex.Match(linkHeader, @"<([^>]+)>;\s*rel=""next""");
            if (match.Success)
                return match.Groups[1].Value;
        }

        return null; // No hay más páginas
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
        var canvasAuthData = _sessionStorageService.Retrieve<CanvasAuthToken>(CanvasAuthToken.Key);
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
        for (var attempt = 1; attempt <= MaxRetries; attempt++)
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
            if (float.TryParse(values.FirstOrDefault(), out var newQuota))
                _sessionStorageService.Store("X-Rate-Limit-Remaining", newQuota);

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

    private string EnsurePerPageInUrl(string url, int perPageValue)
    {
        // Si la URL ya contiene `per_page=`, no hacemos nada
        if (url.Contains("per_page="))
            return url;
        
        // Crear una URI absoluta temporal si la URL es relativa
        Uri uri = Uri.TryCreate(url, UriKind.Absolute, out var absoluteUri)
            ? absoluteUri
            : new Uri(new Uri(_appSettings.Canvas.ApiBaseUrl), url); // Se usa una URL base temporal

        // Determinar si agregamos `?per_page` o `&per_page`
        char separator = uri.Query.Length > 0 ? '&' : '?';
        return $"{url}{separator}per_page={perPageValue}";
    }

    private async Task HandleRateLimitAsync(HttpResponseMessage response, int attempt,
        CancellationToken cancellationToken)
    {
        const int baseDelayMilliseconds = 500;
        var delay = baseDelayMilliseconds * Math.Pow(2, attempt); // Exponential backoff

        if (response.Headers.Contains("X-Rate-Limit-Reset-After"))
            if (double.TryParse(response.Headers.GetValues("X-Rate-Limit-Reset-After").FirstOrDefault(),
                    out var resetAfter))
                delay = Math.Max(delay, resetAfter * 1000); // Convert seconds to milliseconds

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
        if (remainingQuota <= 0) return 15000; // Hard pause for no quota remaining

        // Scale the delay dynamically based on remaining quota
        return (int)((1 - remainingQuota / 700) * 1000 * 10);
    }
}