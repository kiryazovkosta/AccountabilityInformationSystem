using FluentValidation;

namespace AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.Create;

public sealed class CreateExciseNomenclatureValidator<TCreateRequest> : AbstractValidator<TCreateRequest>
    where TCreateRequest : CreateExciseNomenclatureRequest
{
    public CreateExciseNomenclatureValidator(string codePattern, int descriptionMaxLength)
    {
        RuleFor(x => x.Code)
            .Matches(codePattern)
            .WithMessage($"Code must to be into {codePattern} format.");

        RuleFor(x => x.DescriptionBg)
            .NotEmpty()
            .MaximumLength(descriptionMaxLength)
            .WithMessage($"DescriptionBg must not exceed {descriptionMaxLength} characters.");

        RuleFor(x => x.DescriptionEn)
            .NotEmpty()
            .MaximumLength(descriptionMaxLength)
            .WithMessage($"DescriptionEn must not exceed {descriptionMaxLength} characters.");
    }
}
