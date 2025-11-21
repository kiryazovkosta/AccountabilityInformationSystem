using AccountabilityInformationSystem.Api.Entities.Excise;
using AccountabilityInformationSystem.Api.Entities.Flow;
using AccountabilityInformationSystem.Api.Models.Flow.Ikunks;
using AccountabilityInformationSystem.Api.Services.Sorting;

namespace AccountabilityInformationSystem.Api.Models.ExciseNomenclatures;

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
}
