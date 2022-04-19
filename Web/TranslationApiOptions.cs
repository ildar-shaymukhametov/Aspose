using System.ComponentModel.DataAnnotations;

namespace Web;

public class TranslationApiOptions
{
    [Url]
    [Required]
    public string? Url { get; set; }

    [Required]
    public string? ApiKey { get; set; }
}