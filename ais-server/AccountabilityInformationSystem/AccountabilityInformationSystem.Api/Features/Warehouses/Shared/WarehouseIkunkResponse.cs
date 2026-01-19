namespace AccountabilityInformationSystem.Api.Features.Warehouses.Shared;

public sealed record WarehouseIkunkResponse
{
    public string Id { get; set; }
    public string FullName { get; set; }
    public List<WarehouseIkunkMeasurementPointResponse> MeasurementPoints { get; init; }
}
