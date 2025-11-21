namespace AccountabilityInformationSystem.Api.Models.Flow.MeasurementPointsData;

public sealed record MeasurementPointDataMeasurementPointIkunkResponse
{
    public string Id { get; init; }
    public string FullName { get; set; }
    public MeasurementPointDataMeasurementPointIkunkWarehouseResponse Warehouse { get; init; }
}
