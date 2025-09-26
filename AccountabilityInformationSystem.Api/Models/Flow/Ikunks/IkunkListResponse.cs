using AccountabilityInformationSystem.Api.Entities.Flow;
using AccountabilityInformationSystem.Api.Models.Flow.MeasurementPoints;

namespace AccountabilityInformationSystem.Api.Models.Flow.Ikunks;

public sealed record IkunkListResponse
{
    public string Id { get; set; }
    public string FullName { get; set; }
    public List<MeasurementPointListResponse> MeasurementPoints { get; set; }
}
