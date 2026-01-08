namespace AccountabilityInformationSystem.Api.Models.ExciseNomenclatures;

public record CreateExciseNomenclatureRequest
{
    public string Code { get; set; }
    public string BgDescription { get; set; }
    public string? DescriptionEn { get; set; }
    public bool IsUsed { get; set; }
}

public sealed record CreateApCodeNomenclatureRequest : CreateExciseNomenclatureRequest;

public sealed record CreateBrandNameNomenclatureRequest : CreateExciseNomenclatureRequest;

public sealed record CreateCnCodeNomenclatureRequest : CreateExciseNomenclatureRequest;

public sealed record CreateExciseNomenclatureBatchRequest<TCreateRequest>
    where TCreateRequest : CreateExciseNomenclatureRequest
{
    public required List<TCreateRequest> Entries { get; init; }
}
