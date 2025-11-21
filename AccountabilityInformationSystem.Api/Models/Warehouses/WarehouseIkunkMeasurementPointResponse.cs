namespace AccountabilityInformationSystem.Api.Models.Warehouses;

public sealed record WarehouseIkunkMeasurementPointResponse
{
    public string Id { get; init; }
    public string FullName { get; set; }
    public string ControlPoint { get; init; }
}
