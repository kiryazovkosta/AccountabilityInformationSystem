using AccountabilityInformationSystem.Api.Entities.Flow;
using Microsoft.AspNetCore.Mvc;

namespace AccountabilityInformationSystem.Api.Models.Flow.MeasurementPoints;

public sealed record MeasuringPointsQueryParameters
{
    [FromQuery(Name = "q")]
    public string? Search { get; set; }
    public FlowDirectionType? Direction { get; init; }
    public TransportType? Transport { get; init; }
    public string? Sort { get; init; }
}
