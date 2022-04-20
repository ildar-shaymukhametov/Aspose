namespace Contracts;

public class TranslationRequest
{
    public string[] Texts { get; set; }
    public string TargetLanguageCode { get; set; }
    public string SourceLanguageCode { get; set; }
}