namespace AccountabilityInformationSystem.Api.Models.Warehouses;

public sealed record WarehouseListResponse
{
    public string Id { get; init; }
    public string FullName { get; set; }
}
