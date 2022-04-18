try
{
    if (args.Length < 3)
    {
        Console.WriteLine("Required arguments not provided");
        return;
    }

    var filename = args[0];

    using var httpClient = new HttpClient
    {
        BaseAddress = new Uri("https://localhost:5001/translate")
    };
    httpClient.DefaultRequestHeaders.Add("X-Translation-Api-Key", "***");

    var translationClient = new TranslationApiClient(httpClient);
    var texts = new [] { "house" };
    var sourceLanguage = args[1];
    var targetLanguage = args[2];

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
    Console.WriteLine($"Error: {ex.GetBaseException()?.Message}");
}
