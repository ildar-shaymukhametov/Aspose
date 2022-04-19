using System.ComponentModel.DataAnnotations;

public class TransaleFileViewModel : IValidatableObject
{
    const int MaxFileSizeBytes = 10485760;
    readonly string _allowedExtensions = "docx,doc";

    public IFormFile File { get; set; }

    [StringLength(2)]
    public string SourceLanguage { get; set; }

    [StringLength(2)]
    public string TargetLanguage { get; set; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (File?.Length > MaxFileSizeBytes)
        {
            yield return new ValidationResult($"Размер файла не должен превышать {MaxFileSizeBytes / 1024} мегабайт", new[] { nameof(File) });
        }

        var attribute = new FileExtensionsAttribute() { Extensions = _allowedExtensions };
        if (!attribute.IsValid(File?.FileName))
        {
            yield return new ValidationResult($"Неподдерживаемый формат файла", new[] { nameof(File) });
        }
    }
}