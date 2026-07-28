using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Shared.Services.Mapping;

namespace AccountabilityInformationSystem.Api.Features.Flow.Ikunks.Create;

public sealed record CreateIkunkRequest : IMapTo<Ikunk>, IMapCustom
{
    public required string Name { get; init; }
    public required string FullName { get; init; }
    public string? Description { get; init; }
    public required int OrderPosition { get; init; }
    public required DateOnly ActiveFrom { get; init; }
    public required DateOnly ActiveTo { get; init; }
    public required string WarehouseId { get; init; }

    public void CreateMappings(Mapster.TypeAdapterConfig config) =>
        config.NewConfig<CreateIkunkRequest, Ikunk>()
            .Map(dest => dest.Id, _ => $"ik_{Guid.CreateVersion7()}");
}
