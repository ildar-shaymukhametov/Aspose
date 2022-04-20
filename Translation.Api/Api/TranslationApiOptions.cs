using System.ComponentModel.DataAnnotations;

namespace Translation.Api;

public class TranslationApiOptions
{
    [Url]
    [Required]
    public string? TranslateUrl { get; set; }

    [Url]
    [Required]
    public string? LanguagesUrl { get; set; }

    [Required]
    public string? FolderId { get; set; }
}