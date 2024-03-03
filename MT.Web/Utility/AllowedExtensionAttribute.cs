using System.ComponentModel.DataAnnotations;

namespace MT.Web.Utility;

public class AllowedExtensionAttribute : ValidationAttribute
{
    private readonly string[] _allowedExtension;
    public AllowedExtensionAttribute(string[] extensions)
    {
        _allowedExtension = extensions;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is IFormFile file)
        {
            var extension = Path.GetExtension(file.FileName);
            if (!_allowedExtension.Contains(extension.ToLower()))
            {
                return new ValidationResult("Invalid image extension.\n Allowed extensions are (" + string.Join(", ", _allowedExtension) + ")");
            }
        }
        return ValidationResult.Success;
    }
}