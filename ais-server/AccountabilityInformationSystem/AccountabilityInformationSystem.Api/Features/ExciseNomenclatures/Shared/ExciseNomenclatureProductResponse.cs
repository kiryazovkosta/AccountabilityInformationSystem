namespace AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared;

public record ExciseNomenclatureProductResponse
{
    public string Id { get; init; }
    public string Code { get; init; }
    public string FullName { get; init; }
}
