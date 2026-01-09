using FluentValidation;

namespace AccountabilityInformationSystem.Api.Models.ExciseNomenclatures;

public sealed class CreateExciseNomenclatureValidator<TCreateRequest> : AbstractValidator<TCreateRequest>
    where TCreateRequest : CreateExciseNomenclatureRequest
{
    public CreateExciseNomenclatureValidator(string codePattern, int descriptionMaxLength)
    {
        RuleFor(x => x.Code)
            .Matches(codePattern)
            .WithMessage($"Code must to be into {codePattern} format.");

        RuleFor(x => x.BgDescription)
            .NotEmpty()
            .MaximumLength(descriptionMaxLength)
            .WithMessage($"BgDescription must not exceed {descriptionMaxLength} characters.");

        RuleFor(x => x.DescriptionEn)
            .MaximumLength(descriptionMaxLength)
            .When(x => !string.IsNullOrEmpty(x.DescriptionEn))
            .WithMessage($"DescriptionEn must not exceed {descriptionMaxLength} characters.");
    }
}
