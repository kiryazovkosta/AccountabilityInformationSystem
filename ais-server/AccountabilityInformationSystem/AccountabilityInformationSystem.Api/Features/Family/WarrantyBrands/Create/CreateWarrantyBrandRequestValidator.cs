using AccountabilityInformationSystem.Api.Shared.Constants;
using FluentValidation;

namespace AccountabilityInformationSystem.Api.Features.Family.WarrantyBrands.Create;

internal sealed class CreateWarrantyBrandRequestValidator : AbstractValidator<CreateWarrantyBrandRequest>
{
    public CreateWarrantyBrandRequestValidator()
    {
        RuleFor(wb => wb.Name)
            .NotEmpty()
            .MaximumLength(EntitiesConstants.NameMaxLength)
            .WithMessage($"Name must not exceed {EntitiesConstants.NameMaxLength} characters.");

        RuleFor(wb => wb.Logo)
            .MaximumLength(EntitiesConstants.WarrantyBrand.LogoMaxLength)
            .When(wr => string.IsNullOrEmpty(wr.Logo))
            .WithMessage($"When Logo is provided it must not exceed {EntitiesConstants.WarrantyBrand.LogoMaxLength} characters.");
    }
}
