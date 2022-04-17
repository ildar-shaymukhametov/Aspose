using System.Net.Http.Json;

public interface ITranslationApiClient
{
    Task<string?[]?> TranslateAsync(TranslationRequest request);
}

public class TranslationApiClient : ITranslationApiClient
{
    private readonly HttpClient _httpClient;

    public TranslationApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string?[]?> TranslateAsync(TranslationRequest request)
    {
        var content = JsonContent.Create(request);
        var response = await _httpClient.PostAsync("translate", content);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<TranslationResponse>();
        var translations = result?.Translations?.Select(x => x.Text).ToArray();

        return translations;
    }
}
