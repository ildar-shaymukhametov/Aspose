using Client;

var filename = GetInput("Укажите полный путь к файлу:");
var sourceLanguage = GetInput("Укажите язык источника:");
var targetLanguage = GetInput("Укажите язык назначения:");

try
{
    await using var file = File.Open(filename, FileMode.Open);
    var texts = new Parser.Parser().Parse(file);
    if (!texts.Any())
    {
        Console.WriteLine("File contains no text");
        return;
    }

    var client = CreateClient();
    var translations = await client.TranslateAsync(texts, sourceLanguage, targetLanguage);
    if (translations == null)
    {
        Console.WriteLine($"Unable to translate text \"{string.Join(", ", texts)}\" from {sourceLanguage} to {targetLanguage}");
        return;
    }

    foreach (var item in translations)
    {
        Console.WriteLine(item);
    }
}
catch (Exception ex)
{
    Console.WriteLine("Failed to translate file");
    Console.Error.WriteLine($"Error: {ex}");
}

static TranslationApiClient CreateClient()
{
    var httpClient = new HttpClient
    {
        BaseAddress = new Uri("https://localhost:5001/translate")
    };
    httpClient.DefaultRequestHeaders.Add("X-Translation-Api-Key", "***");
    var translationClient = new TranslationApiClient(httpClient);

    return translationClient;
}

string GetInput(string prompt)
{
    string? result = null;
    while (string.IsNullOrWhiteSpace(result))
    {
        Console.WriteLine(prompt);
        result = Console.ReadLine();
    }

    return result;
}