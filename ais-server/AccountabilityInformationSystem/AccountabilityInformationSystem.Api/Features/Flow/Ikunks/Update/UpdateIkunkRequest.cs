namespace AccountabilityInformationSystem.Api.Features.Flow.Ikunks.Update;

public sealed record UpdateIkunkRequest
{
    internal string Id { get; init; }
    public string? Name { get; init; }
    public string? FullName { get; init; }
    public string? Description { get; init; }
    public int? OrderPosition { get; init; }
    public DateOnly? ActiveFrom { get; init; }
    public DateOnly? ActiveTo { get; init; }
    public string? WarehouseId { get; init; }
}
