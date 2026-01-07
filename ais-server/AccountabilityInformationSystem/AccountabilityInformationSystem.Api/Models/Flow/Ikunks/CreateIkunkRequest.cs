namespace AccountabilityInformationSystem.Api.Models.Flow.Ikunks;

public sealed record CreateIkunkRequest
{
    public required string Name { get; init; }
    public required string FullName { get; init; }
    public string? Description { get; init; }
    public required int OrderPosition { get; init; }
    public required DateOnly ActiveFrom { get; init; }
    public required DateOnly ActiveTo { get; init; }
    public required string WarehouseId { get; init; }
}
