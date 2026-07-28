using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Shared.Services.Mapping;

namespace AccountabilityInformationSystem.Api.Features.Warehouses.Shared;

public sealed record WarehouseResponse : IMapFrom<Warehouse>, IMapCustom
{
    public string Id { get; init; }
    public string Name { get; init; }
    public string FullName { get; init; }
    public string? Description { get; init; }
    public int OrderPosition { get; init; }
    public string ExciseNumber { get; init; }
    public DateOnly ActiveFrom { get; init; }
    public DateOnly ActiveTo { get; init; }
    public List<WarehouseIkunkResponse> Ikunks { get; init; }

    public void CreateMappings(Mapster.TypeAdapterConfig config) =>
        config.NewConfig<Warehouse, WarehouseResponse>()
            .Map(dest => dest.Ikunks, src => src.Ikunks.OrderBy(ikunk => ikunk.OrderPosition));
}
