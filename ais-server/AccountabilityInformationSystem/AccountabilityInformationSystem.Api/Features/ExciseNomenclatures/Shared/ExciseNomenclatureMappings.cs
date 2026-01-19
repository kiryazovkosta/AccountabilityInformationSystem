using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Excise;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.CreateExciseNomenclature;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.UpdateExciseNomenclature;
using AccountabilityInformationSystem.Api.Shared.Services.Sorting;

namespace AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared;

public static class ExciseNomenclatureMappings
{
    public static readonly SortMappingDefinition<ExciseNomenclatureResponse, ApCode> SortMappingApCode = new()
    {
        Mappings =
        [
            new SortMapping(nameof(ExciseNomenclatureResponse.Id), nameof(ApCode.Id)),
            new SortMapping(nameof(ExciseNomenclatureResponse.Code), nameof(ApCode.Code)),
            new SortMapping(nameof(ExciseNomenclatureResponse.DescriptionEn), nameof(ApCode.DescriptionEn)),
            new SortMapping(nameof(ExciseNomenclatureResponse.BgDescription), nameof(ApCode.BgDescription)),
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
            new SortMapping(nameof(ExciseNomenclatureResponse.BgDescription), nameof(BrandName.BgDescription)),
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
                new SortMapping(nameof(ExciseNomenclatureResponse.BgDescription), nameof(CnCode.BgDescription)),
                new SortMapping(nameof(ExciseNomenclatureResponse.IsUsed), nameof(CnCode.IsUsed))
        ]
    };


    public static TEntity ToEntity<TEntity>(this CreateExciseNomenclatureRequest request, string userName, string prefix)
        where TEntity : AuditableEntity, IEntity, IExciseEntity, new()
        => new()
        {
            Id = $"{prefix}_{Guid.CreateVersion7()}",
            Code = request.Code,
            BgDescription = request.BgDescription,
            DescriptionEn = request.DescriptionEn,
            IsUsed = request.IsUsed,
            CreatedBy = userName,
            CreatedAt = DateTime.UtcNow
        };

    public static ExciseNomenclatureResponse ToResponse<TEntity>(this TEntity exciseEntity)
        where TEntity : AuditableEntity, IEntity, IExciseEntity
        => new()
        {
            Id = exciseEntity.Id,
            Code = exciseEntity.Code,
            BgDescription = exciseEntity.BgDescription,
            DescriptionEn = exciseEntity.DescriptionEn,
            IsUsed = exciseEntity.IsUsed
        };

    public static void UpdateFromRequest<TEntity, TUpdateRequest>(this TEntity entity, TUpdateRequest request, string userName)
        where TEntity : AuditableEntity, IEntity, IExciseEntity
        where TUpdateRequest : UpdateExciseNomenclatureRequest
    {
        entity.Code = request.Code ?? entity.Code;
        entity.BgDescription = request.BgDescription ?? entity.BgDescription;
        entity.DescriptionEn = request.DescriptionEn ?? entity.BgDescription;
        entity.IsUsed = request.IsUsed ?? entity.IsUsed;
        entity.ModifiedBy = userName;
        entity.ModifiedAt = DateTime.UtcNow;
    }
}
