using AccountabilityInformationSystem.Api.Shared.Constants;
using FluentValidation;

namespace AccountabilityInformationSystem.Api.Features.ProductTypes.CreateProductType;

public sealed class CreateProductTypeRequestValidator : AbstractValidator<CreateProductTypeRequest>
{
    public CreateProductTypeRequestValidator()
    {
        RuleFor(x => x.Name)
    .NotEmpty()
    .MaximumLength(EntitiesConstants.NameMaxLength)
    .WithMessage($"Name must not exceed {EntitiesConstants.NameMaxLength} characters.");

        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(EntitiesConstants.FullNameMaxLength)
            .WithMessage($"FullName must not exceed {EntitiesConstants.FullNameMaxLength} characters.");
    }
}
