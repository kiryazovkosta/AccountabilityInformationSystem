using AccountabilityInformationSystem.Api.Shared.Constants;
using FluentValidation;
using AccountabilityInformationSystem.Api.Shared.Extensions;

namespace AccountabilityInformationSystem.Api.Features.Family.WarrantyRecords.Create;

internal sealed class CreateWarrantyRecordRequestValidator : AbstractValidator<CreateWarrantyRecordRequest>
{
    public CreateWarrantyRecordRequestValidator()
    {
        RuleFor(x => x.WarrantyBrandId)
            .NotEmpty()
            .MaximumLength(EntitiesConstants.IdMaxLength);

        RuleFor(x => x.Model)
            .NotEmpty()
            .MaximumLength(EntitiesConstants.WarrantyRecord.ModelMaxLength)
            .WithMessage($"Model must not exceed {EntitiesConstants.WarrantyRecord.ModelMaxLength} characters.");

        RuleFor(x => x.PurchaseDate)
            .NotEmpty();

        RuleFor(x => x.Receipt)
            .MaxFileSize(2 * 1024 * 1024)
            .AllowedExtensions([".jpg", ".jpeg", ".png", ".pdf"])
            .AllowedContentTypes(["image/jpeg", "image/png", "application/pdf"]);

        RuleFor(x => x.FrontImage)
            .MaxFileSize(2 * 1024 * 1024)
            .AllowedExtensions([".jpg", ".jpeg", ".png"])
            .AllowedContentTypes(["image/jpeg", "image/png"]);

        RuleFor(x => x.BackImage)
            .MaxFileSize(2 * 1024 * 1024)
            .AllowedExtensions([".jpg", ".jpeg", ".png" ])
            .AllowedContentTypes(["image/jpeg", "image/png"]);
    }
}
