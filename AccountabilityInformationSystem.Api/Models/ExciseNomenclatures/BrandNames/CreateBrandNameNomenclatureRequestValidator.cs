using AccountabilityInformationSystem.Api.Common.Constants;
using FluentValidation;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace AccountabilityInformationSystem.Api.Models.ExciseNomenclatures.BrandNames;

public sealed class CreateBrandNameNomenclatureRequestValidator : AbstractValidator<CreateBrandNameNomenclatureRequest>
{
    public CreateBrandNameNomenclatureRequestValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .Length(EntitiesConstants.BrandNameConstants.CodeLength)
            .WithMessage($"Code must be exactlly {EntitiesConstants.BrandNameConstants.CodeLength} characters.");

        RuleFor(x => x.Code)
            .NotEmpty()
            .Matches(EntitiesConstants.BrandNameConstants.CodePattern)
            .WithMessage($"Code must to be into {EntitiesConstants.BrandNameConstants.CodePattern} format.");

        RuleFor(x => x.BgDescription)
            .NotEmpty()
            .MaximumLength(EntitiesConstants.BrandNameConstants.DescriptionMaxlength)
            .WithMessage($"BgDescription must not exceed {EntitiesConstants.BrandNameConstants.DescriptionMaxlength} characters.");

        RuleFor(x => x.DescriptionEn)
            .MaximumLength(EntitiesConstants.BrandNameConstants.DescriptionMaxlength)
            .When(x => !string.IsNullOrEmpty(x.DescriptionEn))
            .WithMessage($"DescriptionEn must not exceed {EntitiesConstants.BrandNameConstants.DescriptionMaxlength} characters.");
    }
}
