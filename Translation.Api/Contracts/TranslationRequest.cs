using System.ComponentModel.DataAnnotations;

namespace Contracts;

public class TranslationRequest
{
    [Required]
    public string[]? Texts { get; set; }

    [Required]
    public string? TargetLanguageCode { get; set; }
    public string? SourceLanguageCode { get; set; }
}