using System.Net.Http.Json;
using Contracts;

namespace Client;

public interface ITranslationApiClient
{
    Task<string?[]?> TranslateAsync(string[] texts, string sourceLanguage, string targetLanguage);
    Task<Language[]?> LanguagesAsync();
}

public class TranslationApiClient : ITranslationApiClient
{
    private readonly HttpClient _httpClient;

    public TranslationApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<string?[]?> TranslateAsync(string[] texts, string sourceLanguage, string targetLanguage)
    {
        var content = JsonContent.Create(new TranslationRequest
        {
            Texts = texts,
            SourceLanguageCode = sourceLanguage,
            TargetLanguageCode = targetLanguage
        });
        var response = await _httpClient.PostAsync("translate", content);
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<TranslationResponse>();
        var translations = result?.Translations?.Select(x => x.Text).ToArray();

        return translations;
    }

    public async Task<Language[]?> LanguagesAsync()
    {
        var response = await _httpClient.GetAsync("languages");
        response.EnsureSuccessStatusCode();

        var result = await response.Content.ReadFromJsonAsync<LanguagesResponse>();
        var languages = result?.Languages?.Where(x => !string.IsNullOrWhiteSpace(x.Name)).ToArray();

        return languages;
    }
}