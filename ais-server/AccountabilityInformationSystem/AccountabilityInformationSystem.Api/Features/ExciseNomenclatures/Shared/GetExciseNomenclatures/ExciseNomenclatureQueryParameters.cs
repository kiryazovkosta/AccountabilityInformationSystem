using AccountabilityInformationSystem.Api.Shared.Models;

namespace AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.GetExciseNomenclatures;

public sealed record ExciseNomenclatureQueryParameters : QueryParameters
{
    public bool? IsUsed { get; init; }
}
