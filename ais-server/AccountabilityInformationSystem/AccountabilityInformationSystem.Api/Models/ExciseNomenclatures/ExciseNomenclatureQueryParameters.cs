using AccountabilityInformationSystem.Api.Models.Common;

namespace AccountabilityInformationSystem.Api.Models.ExciseNomenclatures;

public sealed record ExciseNomenclatureQueryParameters : QueryParameters
{
    public bool? IsUsed { get; init; }
}
