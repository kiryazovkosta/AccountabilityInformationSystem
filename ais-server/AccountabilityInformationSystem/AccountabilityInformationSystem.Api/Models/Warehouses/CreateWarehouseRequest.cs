namespace AccountabilityInformationSystem.Api.Models.Warehouses;

public sealed record CreateWarehouseRequest
{
    public required string Name { get; init; }
    public required string FullName { get; init; }
    public string? Description { get; init; }
    public required int OrderPosition { get; init; }
    public required string ExciseNumber { get; init; }
    public required DateOnly ActiveFrom { get; init; }
    public required DateOnly ActiveTo { get; init; }
}
