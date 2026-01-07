using AccountabilityInformationSystem.Api.Common.Constants;
using FluentValidation;

namespace AccountabilityInformationSystem.Api.Models.Warehouses;

internal sealed class CreateWarehouseRequestValidator : AbstractValidator<CreateWarehouseRequest>
{
    public CreateWarehouseRequestValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(EntitiesConstants.NameMaxLength)
            .WithMessage($"Name must not exceed {EntitiesConstants.NameMaxLength} characters.");

        RuleFor(x => x.FullName)
            .NotEmpty()
            .MaximumLength(EntitiesConstants.FullNameMaxLength)
            .WithMessage($"FullName must not exceed {EntitiesConstants.FullNameMaxLength} characters.");

        RuleFor(x => x.Description)
            .MaximumLength(EntitiesConstants.DescriptionMaxLength)
            .When(x => !string.IsNullOrEmpty(x.Description))
            .WithMessage($"Description must not exceed {EntitiesConstants.DescriptionMaxLength} characters.");

        RuleFor(x => x.OrderPosition)
            .GreaterThanOrEqualTo(EntitiesConstants.OrderPositionMinValue)
            .WithMessage($"OrderPosition must be at least {EntitiesConstants.OrderPositionMinValue}.");

        RuleFor(x => x.ExciseNumber)
            .NotEmpty()
            .Matches(EntitiesConstants.ExciseNumberPattern)
            .WithMessage($"ExciseNumber must match the pattern {EntitiesConstants.ExciseNumberPattern}.");

        RuleFor(x => x.ActiveFrom)
            .LessThan(x => x.ActiveTo)
            .WithMessage("ActiveFrom must be earlier than ActiveTo.");

        RuleFor(x => x.ActiveTo)
            .Must(date => date > DateOnly.FromDateTime(DateTime.Today))
            .WithMessage("ActiveTo must be a future date.");
    }
}
