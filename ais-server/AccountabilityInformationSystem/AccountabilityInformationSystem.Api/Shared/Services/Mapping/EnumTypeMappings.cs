using AccountabilityInformationSystem.Api.Domain.Entities.Flow;
using AccountabilityInformationSystem.Api.Shared.Extensions;
using AccountabilityInformationSystem.Api.Shared.Models;
using Mapster;

namespace AccountabilityInformationSystem.Api.Shared.Services.Mapping;

public sealed class EnumTypeMappings : IMapCustom
{
    public void CreateMappings(TypeAdapterConfig config)
    {
        config.NewConfig<FlowDirectionType, EnumTypeResponse>()
            .Map(dest => dest.Value, src => src)
            .Map(dest => dest.Description, src => src.GetDescription());

        config.NewConfig<TransportType, EnumTypeResponse>()
            .Map(dest => dest.Value, src => src)
            .Map(dest => dest.Description, src => src.GetDescription());
    }
}
