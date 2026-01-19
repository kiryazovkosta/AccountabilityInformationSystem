namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPointsData.Shared;

public sealed record MeasurementPointDataMeasurementPointIkunkResponse
{
    public string Id { get; init; }
    public string FullName { get; set; }
    public MeasurementPointDataMeasurementPointIkunkWarehouseResponse Warehouse { get; init; }
}
