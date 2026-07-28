using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Shared.Extensions;
using AccountabilityInformationSystem.Api.Shared.Models;
using AccountabilityInformationSystem.Api.Shared.Services.Mapping;

namespace AccountabilityInformationSystem.Api.Features.Flow.MeasurementPoints.Shared;

public sealed record MeasurementPointResponse : ILinksResponse, IMapFrom<MeasurementPoint>, IMapCustom
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
    public MeasurementPointIkunkResponse? Ikunk { get; init; }
    public List<LinkResponse> Links { get; set; }

    public void CreateMappings(Mapster.TypeAdapterConfig config) =>
        config.NewConfig<MeasurementPoint, MeasurementPointResponse>()
            .Map(dest => dest.FlowDirection, src => new EnumTypeResponse
            {
                Value = src.FlowDirection,
                Description = src.FlowDirection.GetDescription()
            })
            .Map(dest => dest.Transport, src => new EnumTypeResponse
            {
                Value = src.Transport,
                Description = src.Transport.GetDescription()
            });
}
