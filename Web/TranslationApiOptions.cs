using System.ComponentModel.DataAnnotations;

public class TranslationApiOptions
{
    [Url]
    [Required]
    public string? Url { get; set; }

    [Required]
    public string? ApiKey { get; set; }
}