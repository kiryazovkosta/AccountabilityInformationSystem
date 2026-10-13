namespace AccountabilityInformationSystem.Api.Features.ExciseNoms.Shared.Create;

public record CreateExciseNomenclatureRequest
{
    public string Code { get; set; }
    public string DescriptionBg { get; set; }
    public string DescriptionEn { get; set; }
    public bool IsUsed { get; set; }
}
