using AccountabilityInformationSystem.Api.Domain.Entities.Family.Warranty;
using AccountabilityInformationSystem.Api.Features.Family.WarrantyBrands.Create;
using AccountabilityInformationSystem.Api.Shared.Services.Mapping;

namespace AccountabilityInformationSystem.Api.Features.Family.WarrantyBrands.Shared;

public sealed record WarrantyBrandResponse : IMapFrom<WarrantyBrand>, IMapCustom
{
    public required string Id { get; init; }
    public required string Name { get; init; }
    public string? Logo { get; init; }
    public List<string> Warranties { get; init; }

    public void CreateMappings(Mapster.TypeAdapterConfig config)
    {
        config.NewConfig<WarrantyBrand, WarrantyBrandResponse>()
            .Map(dest => dest.Warranties, src => src.WarrantyRecords.Select(wr => wr.Model).AsEnumerable().ToList());
    }
}
