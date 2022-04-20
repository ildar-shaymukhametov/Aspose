using Microsoft.Extensions.Options;

namespace Translation.Api.Services;

public class RefreshTokenResponse
{
    public string? IamToken { get; set; }
    public DateTime? ExpiresAt { get; set; }
}

public interface ITokenService
{
    Task<string?> GetRefreshTokenAsync();
}

public class TokenService : ITokenService
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<TokenService> _logger;
    private readonly RefreshTokenApiOptions _options;

    public TokenService(IHttpClientFactory httpClientFactory, IOptions<RefreshTokenApiOptions> options, ILogger<TokenService> logger)
    {
        _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _options = options.Value;
    }

    public async Task<string?> GetRefreshTokenAsync()
    {
        var httpClient = _httpClientFactory.CreateClient("RefreshTokenApi");
        var content = JsonContent.Create(new
        {
            yandexPassportOauthToken = _options.AccessToken
        });

        try
        {
            var response = await httpClient.PostAsync(string.Empty, content);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<RefreshTokenResponse>();
            return result?.IamToken;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get refresh token");
            return null;
        }
    }
}