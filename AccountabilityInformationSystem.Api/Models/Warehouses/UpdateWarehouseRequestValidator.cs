using AccountabilityInformationSystem.Api.Common.Constants;
using FluentValidation;

namespace AccountabilityInformationSystem.Api.Models.Warehouses;

internal sealed class UpdateWarehouseRequestValidator : AbstractValidator<UpdateWarehouseRequest>
{
    public UpdateWarehouseRequestValidator()
    {
        When(req => req.Name is not null, () =>
        {
            RuleFor(req => req.Name)
                .MaximumLength(EntitiesConstants.NameMaxLength)
                .WithMessage($"Name must not exceed {EntitiesConstants.NameMaxLength} characters.");
        });

        When(req => req.FullName is not null, () =>
        {
            RuleFor(req => req.FullName)
                .MaximumLength(EntitiesConstants.FullNameMaxLength)
                .WithMessage($"FullName must not exceed {EntitiesConstants.FullNameMaxLength} characters.");
        });

        When(req => req.Description is not null, () =>
        {
            RuleFor(req => req.Description)
                .MaximumLength(EntitiesConstants.DescriptionMaxLength)
                .WithMessage($"Description must not exceed {EntitiesConstants.DescriptionMaxLength} characters.");
        });

        When(req => req.OrderPosition is not null, () =>
        {
            RuleFor(req => req.OrderPosition)
                .GreaterThanOrEqualTo(EntitiesConstants.OrderPositionMinValue)
                .WithMessage($"OrderPosition must be greater than or equal to {EntitiesConstants.OrderPositionMinValue}.");
        });

        When(req => req.ExciseNumber is not null, () =>
        {
            RuleFor(req => req.ExciseNumber)
                .Matches(EntitiesConstants.ExciseNumberPattern)
                .WithMessage($"ExciseNumber must match the required {EntitiesConstants.ExciseNumberPattern} pattern.");
        });

        When(req => req.ActiveFrom is not null && req.ActiveTo is not null, () =>
        {
            RuleFor(req => req.ActiveFrom)
                .LessThan(req => req.ActiveTo)
                .WithMessage("ActiveFrom must be earlier than ActiveTo.");
        });
        When(req => req.ActiveTo is not null, () =>
        {
            RuleFor(req => req.ActiveTo)
                .Must(date => date > DateOnly.FromDateTime(DateTime.Today))
                .WithMessage("ActiveTo must be a future date.");
        });
    }
}
