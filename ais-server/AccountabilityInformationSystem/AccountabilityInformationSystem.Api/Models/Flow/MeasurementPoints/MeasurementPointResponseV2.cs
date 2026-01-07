using AccountabilityInformationSystem.Api.Models.Common;
using AccountabilityInformationSystem.Api.Models.Flow.Ikunks;

namespace AccountabilityInformationSystem.Api.Models.Flow.MeasurementPoints;

public sealed record MeasurementPointResponseV2 : ILinksResponse
{
    public string Id { get; init; }
    public string FullName { get; init; }
    public int OrderPosition { get; init; }
    public MeasurementPointIkunkResponse? Ikunk { get; init; }
    public List<LinkResponse> Links { get; set; }
}
