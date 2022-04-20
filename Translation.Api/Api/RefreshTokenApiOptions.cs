using System.ComponentModel.DataAnnotations;

namespace Translation.Api;

public class RefreshTokenApiOptions
{
    [Url]
    [Required]
    public string? Url { get; set; }

    [Required]
    public string? AccessToken { get; set; }
}