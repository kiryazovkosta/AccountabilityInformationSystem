using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Shared.Services.Mapping;

namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.GetAllV2;

public sealed class MeasurementPointIkunkResponseV2 : IMapFrom<Ikunk>
{
    public string Id { get; init; }
    public string Name { get; init; }
}
