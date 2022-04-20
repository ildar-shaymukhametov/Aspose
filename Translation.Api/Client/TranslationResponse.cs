namespace Client;

public class TranslationResponse
{
    public Translation[]? Translations { get; set; }
}

public class Translation
{
    public string? Text { get; set; }
}