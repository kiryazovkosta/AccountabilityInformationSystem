using AccountabilityInformationSystem.Api.Shared.Constants;
using FluentValidation;

namespace AccountabilityInformationSystem.Api.Features.Flow.Ikunks.UpdateIkunk;

internal sealed class UpdateIkunkRequestValidator : AbstractValidator<UpdateIkunkRequest>
{
    public UpdateIkunkRequestValidator()
    {
        When(req => req.Name is not null, () =>
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("'Name' must not be empty.");
            RuleFor(x => x.Name)
                .MaximumLength(EntitiesConstants.NameMaxLength)
                .WithMessage($"'Name' must not exceed {EntitiesConstants.NameMaxLength} characters.");
        });

        When(req => req.FullName is not null, () =>
        {
            RuleFor(x => x.FullName)
                .NotEmpty()
                .WithMessage("'Full Name' must not be empty.");

            RuleFor(x => x.FullName)
                .MaximumLength(EntitiesConstants.NameMaxLength)
                .WithMessage($"'Full Name' must not exceed {EntitiesConstants.FullNameMaxLength} characters.");
        });

        When(req => req.Description is not null, () =>
        {
            RuleFor(x => x.Description)
                .NotEmpty()
                .WithMessage("'Description' must not be empty.");

            RuleFor(x => x.Description)
                .MaximumLength(EntitiesConstants.DescriptionMaxLength)
                .WithMessage($"'Description' must not exceed {EntitiesConstants.DescriptionMaxLength} characters.");
        });

        RuleFor(req => req.OrderPosition)
            .GreaterThanOrEqualTo(EntitiesConstants.OrderPositionMinValue)
            .When(req => req.OrderPosition.HasValue)
            .WithMessage($"'Order Position' must be at least {EntitiesConstants.OrderPositionMinValue}.");

        When(req => req.ActiveFrom.HasValue && req.ActiveTo.HasValue, () =>
        {
            RuleFor(req => req.ActiveFrom)
                .LessThan(req => req.ActiveTo)
                .WithMessage("'Active From' must be earlier than 'Active To'.");
        });

        When(req => req.ActiveTo.HasValue, () =>
        {
            RuleFor(req => req.ActiveTo)
                .Must(date => date > DateOnly.FromDateTime(DateTime.Today))
                .WithMessage("'Active To' must be a future date.");
        });

        When(req => req.WarehouseId is not null, () =>
        {
            RuleFor(x => x.WarehouseId)
                .NotEmpty()
                .WithMessage("'WarehouseId' must not be empty.");
            RuleFor(x => x.WarehouseId)
                .MaximumLength(EntitiesConstants.IdMaxLength)
                .WithMessage($"'WarehouseId' must not exceed {EntitiesConstants.IdMaxLength} characters.");
        });
    }
}
