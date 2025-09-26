namespace AccountabilityInformationSystem.Api.Models.Warehouses;

public sealed record WarehousesCollectionResponse
{
    public List<WarehouseResponse> Items { get; init; }
}
