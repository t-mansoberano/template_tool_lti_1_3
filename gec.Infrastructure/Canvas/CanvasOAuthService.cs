using System.Text.Json;
using CSharpFunctionalExtensions;
using gec.Infrastructure.Canvas.Models;
using gec.Infrastructure.Common;
using Microsoft.Extensions.Configuration;

namespace gec.Infrastructure.Canvas;

public class CanvasOAuthService : ICanvasOAuthService
{
    private readonly string _tokenEndpoint;
    private readonly string _canvasBaseUrl;
    private readonly string _clientId;
    private readonly string _clientSecret;
    private readonly string _redirectUri;
    private readonly HttpClient _httpClient;

    public CanvasOAuthService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        if (configuration == null)
        {
            throw new ArgumentNullException(nameof(configuration), "La configuración no puede ser null.");
        }
        
        _canvasBaseUrl = $"{configuration["Canvas:ApiBaseUrl"]}"
                         ?? throw new InvalidOperationException("Canvas:ApiBaseUrl no está configurado en la configuración.");
        _tokenEndpoint = $"{configuration["Canvas:ApiBaseUrl"]}/login/oauth2/token"
                         ?? throw new InvalidOperationException("Canvas:ApiBaseUrl no está configurado en la configuración.");
        _clientId = configuration["Canvas:ClientId"]
                    ?? throw new InvalidOperationException("Canvas:ClientId no está configurado en la configuración.");
        _clientSecret = configuration["Canvas:ClientSecret"]
                        ?? throw new InvalidOperationException("Canvas:ClientSecret no está configurado en la configuración.");
        _redirectUri = configuration["Canvas:RedirectUri"]
                       ?? throw new InvalidOperationException("Canvas:RedirectUri no está configurado en la configuración.");

        _httpClient = httpClientFactory.CreateClient("CanvasClient");
    }
    
    public string BuildAuthorizationUrl()
    {
        return $"{_canvasBaseUrl}/login/oauth2/auth?" +
               $"client_id={_clientId}&" +
               $"response_type=code&" +
               $"redirect_uri={Uri.EscapeDataString(_redirectUri)}&" +
               $"scope=url:GET|/api/v1/courses/:id url:GET|/api/v1/courses/:course_id/enrollments url:GET|/api/v1/courses/:course_id/folders url:GET|/api/v1/folders/:id/folders url:GET|/api/v1/folders/:id/files url:GET|/api/v1/courses/:course_id/assignments url:GET|/api/v1/courses/:course_id/assignments/:assignment_id/submissions/:user_id url:GET|/api/v1/announcements url:GET|/api/v1/users/self/favorites/courses url:GET|/api/v1/users/:id url:GET|/api/v1/courses url:GET|/api/v1/accounts/:account_id/sub_accounts url:GET|/api/v1/courses/:course_id/todo url:GET|/api/v1/users/:user_id/courses url:GET|/api/v1/courses/:id&" +
               $"&state={Guid.NewGuid()}";
    }

    public async Task<Result<TokenResponse>> HandleOAuthCallbackAsync(Dictionary<string, string> query)
    {
        var oAuthQuery = new OAuthQuery(query);
        var validate = oAuthQuery.Validate();
        if (validate.IsFailure) return Result.Failure<TokenResponse>(validate.Error);
        
        // Construir el cuerpo de la solicitud para el Token Endpoint
        var requestData = new FormUrlEncodedContent(new[]
        {
            new KeyValuePair<string, string>("grant_type", "authorization_code"),
            new KeyValuePair<string, string>("client_id", _clientId),
            new KeyValuePair<string, string>("client_secret", _clientSecret),
            new KeyValuePair<string, string>("redirect_uri", _redirectUri),
            new KeyValuePair<string, string>("code", oAuthQuery.Code)
        });

        // Realizar la solicitud al Token Endpoint
        var response = await _httpClient.PostAsync(_tokenEndpoint, requestData);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync();
            return Result.Failure<TokenResponse>($"Error al obtener el token: {errorContent}");
        }

        var content = await response.Content.ReadAsStringAsync();

        // Opciones de deserialización
        var options = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = new SnakeCaseNamingPolicy()
        };

        var tokenResponse = JsonSerializer.Deserialize<TokenResponse>(content, options);

        return Result.Success(tokenResponse);
    }

}