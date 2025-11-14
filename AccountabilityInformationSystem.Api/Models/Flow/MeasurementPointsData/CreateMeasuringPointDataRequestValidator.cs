using FluentValidation;

namespace AccountabilityInformationSystem.Api.Models.Flow.MeasurementPointsData;

public class CreateMeasuringPointDataRequestValidator : AbstractValidator<CreateMeasuringPointDataRequest>
{
    public CreateMeasuringPointDataRequestValidator()
    {
        RuleFor(x => x.MeasurementPointId).NotEmpty();
        RuleFor(x => x.Number).GreaterThan(0);
        RuleFor(x => x.BeginTime).LessThan(x => x.EndTime);
        RuleFor(x => x.FlowDirectionType).IsInEnum();
        RuleFor(x => x.ProductId).NotEmpty();
    }
}
