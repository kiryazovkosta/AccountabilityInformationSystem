namespace AccountabilityInformationSystem.Api.Features.ExciseNoms.Shared.Update;

public record UpdateExciseNomenclatureRequest
{
    internal string Id { get; init; }
    public string? Code { get; init; }
    public string? DescriptionBg { get; init; }
    public string? DescriptionEn { get; init; }
    public bool? IsUsed { get; init; }
}
