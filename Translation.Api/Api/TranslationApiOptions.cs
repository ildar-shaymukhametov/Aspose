using System.ComponentModel.DataAnnotations;

public class TranslationApiOptions
{
    [Url]
    [Required]
    public string? Url { get; set; }

    [Required]
    public string? FolderId { get; set; }

    [Required]
    public string? Token { get; set; }
}