using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Excise;
using AccountabilityInformationSystem.Api.Features.ExciseNoms.Shared.Create;
using AccountabilityInformationSystem.Api.Features.ExciseNoms.Shared.Update;
using AccountabilityInformationSystem.Api.Shared.Services.Mapping;
using AccountabilityInformationSystem.Api.Shared.Services.Sorting;
using Mapster;

namespace AccountabilityInformationSystem.Api.Features.ExciseNoms.Shared;

public static class ExciseNomenclatureMappings
{
    public static readonly SortMappingDefinition<ExciseNomenclatureResponse, ApCode> SortMappingApCode = new()
    {
        Mappings =
        [
            new SortMapping(nameof(ExciseNomenclatureResponse.Id), nameof(ApCode.Id)),
            new SortMapping(nameof(ExciseNomenclatureResponse.Code), nameof(ApCode.Code)),
            new SortMapping(nameof(ExciseNomenclatureResponse.DescriptionEn), nameof(ApCode.DescriptionEn)),
            new SortMapping(nameof(ExciseNomenclatureResponse.DescriptionBg), nameof(ApCode.DescriptionBg)),
            new SortMapping(nameof(ExciseNomenclatureResponse.IsUsed), nameof(ApCode.IsUsed))
        ]
    };

    public static readonly SortMappingDefinition<ExciseNomenclatureResponse, BrandName> SortMappingBrandName = new()
    {
        Mappings =
        [
            new SortMapping(nameof(ExciseNomenclatureResponse.Id), nameof(BrandName.Id)),
            new SortMapping(nameof(ExciseNomenclatureResponse.Code), nameof(BrandName.Code)),
            new SortMapping(nameof(ExciseNomenclatureResponse.DescriptionEn), nameof(BrandName.DescriptionEn)),
            new SortMapping(nameof(ExciseNomenclatureResponse.DescriptionBg), nameof(BrandName.DescriptionBg)),
            new SortMapping(nameof(ExciseNomenclatureResponse.IsUsed), nameof(BrandName.IsUsed))
        ]
    };

    public static readonly SortMappingDefinition<ExciseNomenclatureResponse, CnCode> SortMappingCnCode = new()
    {
        Mappings =
        [
            new SortMapping(nameof(ExciseNomenclatureResponse.Id), nameof(CnCode.Id)),
                new SortMapping(nameof(ExciseNomenclatureResponse.Code), nameof(CnCode.Code)),
                new SortMapping(nameof(ExciseNomenclatureResponse.DescriptionEn), nameof(CnCode.DescriptionEn)),
                new SortMapping(nameof(ExciseNomenclatureResponse.DescriptionBg), nameof(CnCode.DescriptionBg)),
                new SortMapping(nameof(ExciseNomenclatureResponse.IsUsed), nameof(CnCode.IsUsed))
        ]
    };

    public static TEntity ToEntity<TEntity>(this CreateExciseNomenclatureRequest request, string userName, string prefix)
        where TEntity : AuditableEntity, IEntity, IExciseEntity, new()
    {
        TEntity entity = request.Adapt<TEntity>();
        entity.Id = $"{prefix}_{Guid.CreateVersion7()}";
        entity.CreatedBy = userName;
        entity.CreatedAt = DateTime.UtcNow;
        return entity;
    }

    public static ExciseNomenclatureResponse ToResponse<TEntity>(this TEntity exciseEntity)
        where TEntity : AuditableEntity, IEntity, IExciseEntity
        => exciseEntity.Adapt<ExciseNomenclatureResponse>();

    public static void UpdateFromRequest<TEntity, TUpdateRequest>(this TEntity entity, TUpdateRequest request, string userName)
        where TEntity : AuditableEntity, IEntity, IExciseEntity
        where TUpdateRequest : UpdateExciseNomenclatureRequest
    {
        request.Adapt(entity);
        entity.ModifiedBy = userName;
        entity.ModifiedAt = DateTime.UtcNow;
    }
}

internal sealed class ExciseNomenclatureUpdateMappings : IMapCustom
{
    public void CreateMappings(TypeAdapterConfig config)
    {
        config.NewConfig<UpdateExciseNomenclatureRequest, ApCode>()
            .IgnoreNullValues(true)
            .Ignore(dest => dest.Id);
        config.NewConfig<UpdateExciseNomenclatureRequest, BrandName>()
            .IgnoreNullValues(true)
            .Ignore(dest => dest.Id);
        config.NewConfig<UpdateExciseNomenclatureRequest, CnCode>()
            .IgnoreNullValues(true)
            .Ignore(dest => dest.Id);
    }
}
