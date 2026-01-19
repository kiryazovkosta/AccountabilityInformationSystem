namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.Shared;

public sealed record MeasurementPointListResponse
{
    public string Id { get; init; }
    public string FullName { get; set; }
    public string ControlPoint { get; init; }
    public MeasurementPointSimpleResponse Ikunk { get; init; }
}

public sealed record MeasurementPointSimpleResponse
{
    public string Id { get; init; }
    public string FullName { get; set; }
    public WarehouseSimpleResponse Warehouse { get; init; }
}

public sealed record WarehouseSimpleResponse
{
    public string Id { get; init; }
    public string FullName { get; set; }
}
