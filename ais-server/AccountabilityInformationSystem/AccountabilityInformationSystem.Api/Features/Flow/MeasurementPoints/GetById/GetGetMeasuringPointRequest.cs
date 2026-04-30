namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.GetById;

public sealed record GetGetMeasuringPointRequest(string Id, bool IncludeLinks, string? Fields);
