namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPointsData.GetById;

public sealed record GetMeasuringPointDataRequest(string Id, bool IncludeLinks, string? Fields);
