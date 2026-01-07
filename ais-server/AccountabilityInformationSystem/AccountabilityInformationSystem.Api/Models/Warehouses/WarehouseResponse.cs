using AccountabilityInformationSystem.Api.Models.Flow.Ikunks;
using AccountabilityInformationSystem.Api.Models.Flow.MeasurementPoints;

namespace AccountabilityInformationSystem.Api.Models.Warehouses;

public sealed record WarehouseResponse
{
    public string Id { get; init; }
    public string Name { get; init; }
    public string FullName { get; init; }
    public string? Description { get; init; }
    public int OrderPosition { get; init; }
    public string ExciseNumber { get; init; }
    public DateOnly ActiveFrom { get; init; }
    public DateOnly ActiveTo { get; init; }
    public List<WarehouseIkunkResponse> Ikunks { get; init; }
}
