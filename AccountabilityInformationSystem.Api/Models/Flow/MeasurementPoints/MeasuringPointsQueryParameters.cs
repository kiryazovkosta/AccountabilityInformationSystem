using AccountabilityInformationSystem.Api.Entities.Flow;
using AccountabilityInformationSystem.Api.Models.Common;
using Microsoft.AspNetCore.Mvc;

namespace AccountabilityInformationSystem.Api.Models.Flow.MeasurementPoints;

public abstract record QueryParameters : AcceptHeaderQueryParameter
{
    [FromQuery(Name = "q")]
    public string? Search { get; set; }
    public string? Sort { get; init; }
    public string? Fields { get; init; }
    public int Page { get; init; } = 1;
    public int PageSize { get; init; } = 10;
}

public sealed record MeasuringPointsQueryParameters : QueryParameters
{
    public string? IkunkId { get; init; }
    public FlowDirectionType? FlowDirection { get; init; }
    public TransportType? Transport { get; init; }
}

public sealed record MeasuringPointQueryParameters : AcceptHeaderQueryParameter
{
    public string? Fields { get; init; }
}

public sealed record WarehousesQueryParameters : QueryParameters
{
}

public sealed record IkunkQueryParameters : QueryParameters
{
}
