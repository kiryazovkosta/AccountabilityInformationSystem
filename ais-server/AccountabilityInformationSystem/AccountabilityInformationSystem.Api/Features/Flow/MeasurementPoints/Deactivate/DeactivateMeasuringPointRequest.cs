namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.Deactivate;

public sealed record DeactivateMeasuringPointRequest(string Id, DateOnly ActiveTo);
