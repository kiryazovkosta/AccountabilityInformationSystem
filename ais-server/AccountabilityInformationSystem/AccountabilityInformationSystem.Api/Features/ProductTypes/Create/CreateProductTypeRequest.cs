using AccountabilityInformationSystem.Api.Domain.Entities;
using AccountabilityInformationSystem.Api.Shared.Services.Mapping;
using Mapster;

namespace AccountabilityInformationSystem.Api.Features.ProductTypes.Create;

public sealed record CreateProductTypeRequest : IMapTo<ProductType>, IMapCustom
{
    public required string Name { get; init; }
    public required string FullName { get; init; }

    public void CreateMappings(TypeAdapterConfig config)
    {
        config.NewConfig<CreateProductTypeRequest, ProductType>()
            .Map(dest => dest.Id, _ => $"pt_{Guid.CreateVersion7()}");
    }
}
