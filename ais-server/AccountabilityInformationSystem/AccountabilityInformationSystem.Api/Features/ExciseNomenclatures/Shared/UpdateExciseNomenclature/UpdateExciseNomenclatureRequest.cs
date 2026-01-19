namespace AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.UpdateExciseNomenclature;

public record UpdateExciseNomenclatureRequest
{
    public string? Code { get; init; }
    public string? BgDescription { get; init; }
    public string? DescriptionEn { get; init; }
    public bool? IsUsed { get; init; }
}

public sealed record UpdateApCodeNomenclatureRequest : UpdateExciseNomenclatureRequest;

public sealed record UpdateBrandNameNomenclatureRequest : UpdateExciseNomenclatureRequest;

public sealed record UpdateCnCodeNomenclatureRequest : UpdateExciseNomenclatureRequest;
