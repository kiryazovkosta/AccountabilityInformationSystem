using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Shared.Models;
using AccountabilityInformationSystem.Api.Shared.Services.Mapping;

namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.Shared;

public sealed record MeasurementPointResponseV2 : ILinksResponse, IMapFrom<MeasurementPoint>
{
    public string Id { get; init; }
    public string FullName { get; init; }
    public int OrderPosition { get; init; }
    public MeasurementPointIkunkResponseV2? Ikunk { get; init; }
    public List<LinkResponse> Links { get; set; }
}
