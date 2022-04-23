using System.ComponentModel.DataAnnotations;

namespace Web.Models;

public class TransaleFileViewModel : IValidatableObject
{
    private const int MaxFileSizeBytes = 10485760;

    public IFormFile? File { get; set; }

    [Url]
    public string? Url { get; set; }

    [StringLength(2)]
    public string SourceLanguage { get; set; }

    [StringLength(2)]
    public string TargetLanguage { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Url == null && File == null || Url != null && File != null)
        {
            yield return new ValidationResult($"Необходимо указать либо ссылку, либо файл", new[] { nameof(File), nameof(Url) });
        }
        else if (File != null)
        {
            if (File.Length > MaxFileSizeBytes)
            {
                yield return new ValidationResult($"Размер файла не должен превышать {MaxFileSizeBytes / 1024} мегабайт", new[] { nameof(File) });
            }

            var attribute = new FileExtensionsAttribute { Extensions = string.Join(",", Globals.SupportedExtensions) };
            if (!attribute.IsValid(File.FileName))
            {
                yield return new ValidationResult("Неподдерживаемый формат файла", new[] { nameof(File) });
            }
        }
    }
}