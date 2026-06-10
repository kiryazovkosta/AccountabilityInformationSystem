using FluentValidation;

namespace AccountabilityInformationSystem.Api.Shared.Extensions;

public static class FormFileValidationExtensions
{
    public static IRuleBuilderOptions<T, IFormFile?> MaxFileSize<T>(
        this IRuleBuilder<T, IFormFile?> ruleBuilder, 
        int maxFileSize)
    {
        return ruleBuilder
            .Must(file => file == null || file.Length <= maxFileSize )
            .WithMessage($"The file size cannot exceed {maxFileSize / (1024 * 1024)} MB.");
    }

    public static IRuleBuilderOptions<T, IFormFile?>  AllowedExtensions<T>(
        this IRuleBuilder<T, IFormFile?> ruleBuilder,
        string[] extensions)
    {
        return ruleBuilder
            .Must(file =>
            {
                if (file == null)
                {
                    return true;
                }
                
                string fileExtension = Path.GetExtension(file.FileName).ToLowerInvariant();
                return extensions.Contains(fileExtension);
            })
            .WithMessage($"Invalid format. Allowed extensions: {string.Join(", ", extensions)}");
    }

    public static IRuleBuilderOptions<T, IFormFile?> AllowedContentTypes<T>(
        this IRuleBuilder<T, IFormFile?> ruleBuilder,
        string[] contentTypes)
    {
        return ruleBuilder
            .Must(file =>
            {
                if (file == null)
                {
                    return true;
                }

                string fileContentType = file.ContentType.ToLowerInvariant();
                return contentTypes.Contains(fileContentType);
            })
            .WithMessage($"Invalid content-type. Allowed content types are: {string.Join(", ", contentTypes)}");
    }
}
