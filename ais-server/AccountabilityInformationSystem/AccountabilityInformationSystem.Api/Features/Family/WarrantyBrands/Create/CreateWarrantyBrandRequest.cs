using AccountabilityInformationSystem.Api.Domain.Entities.Family.Warranty;
using AccountabilityInformationSystem.Api.Shared.Services.Mapping;
using Mapster;

namespace AccountabilityInformationSystem.Api.Features.Family.WarrantyBrands.Create;

public sealed record CreateWarrantyBrandRequest : IMapTo<WarrantyBrand>, IMapCustom
{
    public required string Name { get; init; }
    public string? Logo { get; init; }

    public void CreateMappings(TypeAdapterConfig config)
    {
        config.NewConfig<CreateWarrantyBrandRequest, WarrantyBrand>()
            .Map(dest => dest.Id, _ => $"wb_{Guid.CreateVersion7()}");
    }
}
