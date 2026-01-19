using FluentValidation;

namespace AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.UpdateExciseNomenclature;

public sealed class UpdateExciseNomenclatureValidator<TUpdateRequest> : AbstractValidator<TUpdateRequest>
    where TUpdateRequest : UpdateExciseNomenclatureRequest
{
    public UpdateExciseNomenclatureValidator(int codeLength, string codePattern, int descriptionMaxLength)
    {
        RuleFor(x => x.Code)
            .Length(codeLength)
            .When(x => !string.IsNullOrEmpty(x.Code))
            .WithMessage($"Code must be exactlly {codeLength} characters.");

        RuleFor(x => x.Code)
            .Matches(codePattern)
            .When(x => !string.IsNullOrEmpty(x.Code))
            .WithMessage($"Code must to be into {codePattern} format.");

        RuleFor(x => x.BgDescription)
            .MaximumLength(descriptionMaxLength)
            .When(x => !string.IsNullOrEmpty(x.BgDescription))
            .WithMessage($"BgDescription must not exceed {descriptionMaxLength} characters.");

        RuleFor(x => x.DescriptionEn)
            .MaximumLength(descriptionMaxLength)
            .When(x => !string.IsNullOrEmpty(x.DescriptionEn))
            .WithMessage($"DescriptionEn must not exceed {descriptionMaxLength} characters.");
    }
}
