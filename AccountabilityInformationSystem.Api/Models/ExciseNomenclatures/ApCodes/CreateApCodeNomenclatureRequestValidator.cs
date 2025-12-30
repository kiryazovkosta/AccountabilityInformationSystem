using AccountabilityInformationSystem.Api.Common.Constants;
using FluentValidation;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace AccountabilityInformationSystem.Api.Models.ExciseNomenclatures.ApCodes;

public sealed class CreateApCodeNomenclatureRequestValidator : AbstractValidator<CreateApCodeNomenclatureRequest>
{
    public CreateApCodeNomenclatureRequestValidator()
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .Length(EntitiesConstants.ApCodeConstants.CodeLength)
            .WithMessage($"Code must be exactlly {EntitiesConstants.ApCodeConstants.CodeLength} characters.");

        RuleFor(x => x.Code)
            .NotEmpty()
            .Matches(EntitiesConstants.ApCodeConstants.CodePattern)
            .WithMessage($"Code must to be into {EntitiesConstants.ApCodeConstants.CodePattern} format.");

        RuleFor(x => x.BgDescription)
            .NotEmpty()
            .MaximumLength(EntitiesConstants.ApCodeConstants.DescriptionMaxlength)
            .WithMessage($"BgDescription must not exceed {EntitiesConstants.ApCodeConstants.DescriptionMaxlength} characters.");

        RuleFor(x => x.DescriptionEn)
            .MaximumLength(EntitiesConstants.ApCodeConstants.DescriptionMaxlength)
            .When(x => !string.IsNullOrEmpty(x.DescriptionEn))
            .WithMessage($"DescriptionEn must not exceed {EntitiesConstants.ApCodeConstants.DescriptionMaxlength} characters.");
    }
}
