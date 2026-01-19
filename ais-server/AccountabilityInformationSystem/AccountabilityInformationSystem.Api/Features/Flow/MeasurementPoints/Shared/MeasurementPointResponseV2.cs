using AccountabilityInformationSystem.Api.Shared.Models;

namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.Shared;

public sealed record MeasurementPointResponseV2 : ILinksResponse
{
    public string Id { get; init; }
    public string FullName { get; init; }
    public int OrderPosition { get; init; }
    public MeasurementPointIkunkResponse? Ikunk { get; init; }
    public List<LinkResponse> Links { get; set; }
}
