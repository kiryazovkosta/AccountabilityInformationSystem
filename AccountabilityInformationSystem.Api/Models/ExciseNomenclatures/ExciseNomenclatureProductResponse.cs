namespace AccountabilityInformationSystem.Api.Models.Excise;

public record ExciseNomenclatureProductResponse
{
    public string Id { get; init; }
    public string Code { get; init; }
    public string FullName { get; init; }
}
