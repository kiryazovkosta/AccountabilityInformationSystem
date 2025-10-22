using AccountabilityInformationSystem.Api.Models.Flow.MeasurementPoints;
using AccountabilityInformationSystem.Api.Models.Warehouses;

namespace AccountabilityInformationSystem.Api.Models.Flow.Ikunks;

public sealed record IkunkResponseV2
{
    public string Id { get; init; }
    public string Name { get; init; }
    public string FullName { get; init; }
    public string? Description { get; init; }
    public int OrderPosition { get; init; }
    public DateOnly ActiveFrom { get; init; }
    public DateOnly ActiveTo { get; init; }
    public DateTime Created { get; init; }
    public DateTime? Modified { get; init; }
    public WarehouseListResponse? Warehouse { get; init; }
    public List<MeasurementPointListResponse> MeasurementPoints { get; init; }
}
