using AccountabilityInformationSystem.Api.Domain.Entities.Excise;
using AccountabilityInformationSystem.Api.Features.ExciseNoms.Shared.Create;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Constants;

namespace AccountabilityInformationSystem.Api.Features.ExciseNoms.ApCodes.Create;

public sealed class CreateApCodeNomenclatureBatchRequestHandler(ApplicationDbContext dbContext)
    : CreateExciseNomenclatureBatchRequestHandler<ApCode, CreateApCodeNomenclatureRequest>(dbContext)
{
    protected override string EntityIdPrefix => EntitiesConstants.ApCodeConstants.IdPrefix;
}
