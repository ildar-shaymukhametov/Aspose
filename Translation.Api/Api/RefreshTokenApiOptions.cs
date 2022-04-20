using System.ComponentModel.DataAnnotations;

namespace Translation.Api;

public class RefreshTokenApiOptions
{
    [Url]
    public string Url { get; set; }

    public string AccessToken { get; set; }
}