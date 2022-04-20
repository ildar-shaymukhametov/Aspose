namespace Client;

public class LanguagesResponse
{
    public Language[]? Languages { get; set; }
}

public class Language
{
    public string? Code { get; set; }
    public string? Name { get; set; }
}