using AccountabilityInformationSystem.Api.Models.Common;
using AccountabilityInformationSystem.Api.Models.Flow.Ikunks;

namespace AccountabilityInformationSystem.Api.Models.Flow.MeasurementPoints;

public sealed record MeasurementPointResponse : ILinksResponse
{
    public string Id { get; init; }
    public string Name { get; init; }
    public string FullName { get; init; }
    public string? Description { get; init; }
    public string ControlPoint { get; init; }
    public int OrderPosition { get; init; }
    public EnumTypeResponse FlowDirection { get; init; }
    public EnumTypeResponse Transport { get; init; }
    public DateOnly ActiveFrom { get; init; }
    public DateOnly ActiveTo { get; init; }
    public IkunkSimpleResponse? Ikunk { get; init; }
    public List<LinkResponse> Links { get; set; }
}
