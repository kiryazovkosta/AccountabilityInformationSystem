using AccountabilityInformationSystem.Api.Domain.Entities.Excise;
using AccountabilityInformationSystem.Api.Features.ExciseNoms.Shared.Create;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Constants;

namespace AccountabilityInformationSystem.Api.Features.ExciseNoms.CnCodes.Create;

public sealed class CreateCnCodeNomenclatureRequestHandler(ApplicationDbContext dbContext)
    : CreateExciseNomenclatureRequestHandler<CnCode, CreateCnCodeNomenclatureRequest>(dbContext)
{
    protected override string EntityIdPrefix => EntitiesConstants.CnCodeConstants.IdPrefix;
}
