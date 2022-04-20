using Client;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

using var host = Host.CreateDefaultBuilder(args).Build();

var config = new ConfigurationBuilder()
    .AddJsonFile("appsettings.json")
    .AddEnvironmentVariables()
    .Build();

var filename = GetInput("Укажите полный путь к файлу:");
var sourceLanguage = GetInput("Укажите язык источника:");
var targetLanguage = GetInput("Укажите язык назначения:");

Console.WriteLine("Переводим...");

try
{
    await using var file = File.Open(filename, FileMode.Open);
    var texts = new Parser.Parser().Parse(file);
    if (!texts.Any())
    {
        Console.WriteLine("Файл не содержит текст");
        return;
    }

    var client = CreateClient();
    var translations = await client.TranslateAsync(texts, sourceLanguage, targetLanguage);
    if (translations == null)
    {
        Console.WriteLine("Не удалось перевести файл");
        return;
    }

    foreach (var item in translations)
    {
        Console.WriteLine(item);
    }
}
catch (Exception ex)
{
    Console.WriteLine("Не удалось перевести файл");
    Console.Error.WriteLine($"Unable to translate file {filename} from {sourceLanguage} to {targetLanguage}\n{ex}");
}

TranslationApiClient CreateClient()
{
    var settings = config.GetRequiredSection(Settings.SectionName).Get<Settings>();
    var httpClient = new HttpClient
    {
        BaseAddress = settings.Url
    };
    httpClient.DefaultRequestHeaders.Add("X-Translation-Api-Key", settings.ApiKey);
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