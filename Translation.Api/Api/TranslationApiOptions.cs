using System.ComponentModel.DataAnnotations;

namespace Translation.Api;

public class TranslationApiOptions
{
    [Url]
    [Required]
    public string? Url { get; set; }

    [Required]
    public string? FolderId { get; set; }
}