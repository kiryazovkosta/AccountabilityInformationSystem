namespace AccountabilityInformationSystem.Api.Models.Flow.MeasurementPoints;

public class MeasurementPointIkunkResponse
{
    public string Id { get; init; }
    public string Name { get; init; }
    public MeasurementPointIkunkWarehouseResponse Warehouse { get; init; }
}
