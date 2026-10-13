namespace AccountabilityInformationSystem.Api.Features.ExciseNoms.Shared.Create;

public sealed record CreateExciseNomenclatureBatchRequest<TCreateRequest>
    where TCreateRequest : CreateExciseNomenclatureRequest
{
    public required List<TCreateRequest> Entries { get; init; }
}
