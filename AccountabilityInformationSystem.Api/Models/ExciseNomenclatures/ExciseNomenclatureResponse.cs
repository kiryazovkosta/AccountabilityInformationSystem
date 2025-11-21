using AccountabilityInformationSystem.Api.Models.Excise;

namespace AccountabilityInformationSystem.Api.Models.ExciseNomenclatures;

public record ExciseNomenclatureResponse
{
    public string Id { get; init; }
    public string Code { get; set; }
    public string BgDescription { get; set; }
    public string? DescriptionEn { get; set; }
    public bool IsUsed { get; set; }
    public ICollection<ExciseNomenclatureProductResponse> Products { get; set; } = [];
}
