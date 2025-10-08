using AccountabilityInformationSystem.Api.Models.Flow.Ikunks;

namespace AccountabilityInformationSystem.Api.Models.Flow.MeasurementPoints;

public sealed record MeasurementPointsCollectionResponse
{
    public List<MeasurementPointResponse> Items { get; init; }
}
