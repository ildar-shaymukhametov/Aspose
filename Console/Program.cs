try
{
    if (args.Length < 3)
    {
        Console.WriteLine("Required arguments not provided");
        return;
    }

    var filename = args[0];
    using var file = File.Open(filename, FileMode.Open);
    var parser = new Parser();
    var texts = parser.Parse(file);
    if (!texts.Any())
    {
        Console.WriteLine("File contains no text");
        return;
    }

    var sourceLanguage = args[1];
    var targetLanguage = args[2];
    var httpClient = new HttpClient
    {
        BaseAddress = new Uri("https://localhost:5001/translate")
    };
    httpClient.DefaultRequestHeaders.Add("X-Translation-Api-Key", "***");
    var translationClient = new TranslationApiClient(httpClient);
    var translations = await translationClient.TranslateAsync(texts, sourceLanguage, targetLanguage);
    if (translations == null)
    {
        Console.WriteLine($"Unable to translate text: {string.Join(", ", texts)}");
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
