using AccountabilityInformationSystem.Api.Shared.Models;

namespace AccountabilityInformationSystem.Api.Features.ExciseNoms.Shared.GetAll;

public sealed record ExciseNomenclatureQueryParameters : QueryParameters
{
    public bool? IsUsed { get; init; }
}
