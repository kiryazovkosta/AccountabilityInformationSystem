using AccountabilityInformationSystem.Api.Common.Constants;
using FluentValidation;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace AccountabilityInformationSystem.Api.Models.ExciseNomenclatures.CnCodes;

public sealed class CreateCnCodeNomenclatureRequestValidator : AbstractValidator<CreateCnCodeNomenclatureRequest>
{
    public CreateCnCodeNomenclatureRequestValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .Length(EntitiesConstants.CnCodeConstant.CodeLength)
            .WithMessage($"Code must be exactlly {EntitiesConstants.CnCodeConstant.CodeLength} characters.");

        RuleFor(x => x.Code)
            .NotEmpty()
            .Matches(EntitiesConstants.CnCodeConstant.CodePattern)
            .WithMessage($"Code must to be into {EntitiesConstants.CnCodeConstant.CodePattern} format.");

        RuleFor(x => x.BgDescription)
            .NotEmpty()
            .MaximumLength(EntitiesConstants.CnCodeConstant.DescriptionMaxlength)
            .WithMessage($"BgDescription must not exceed {EntitiesConstants.CnCodeConstant.DescriptionMaxlength} characters.");

        RuleFor(x => x.DescriptionEn)
            .MaximumLength(EntitiesConstants.CnCodeConstant.DescriptionMaxlength)
            .When(x => !string.IsNullOrEmpty(x.DescriptionEn))
            .WithMessage($"DescriptionEn must not exceed {EntitiesConstants.CnCodeConstant.DescriptionMaxlength} characters.");
    }
}
