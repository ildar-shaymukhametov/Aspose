using System.ComponentModel.DataAnnotations;

namespace Translation.Api;

public class TranslationApiOptions
{
    [Url]
    public string TranslateUrl { get; set; }

    [Url]
    public string LanguagesUrl { get; set; }

    public string FolderId { get; set; }
}