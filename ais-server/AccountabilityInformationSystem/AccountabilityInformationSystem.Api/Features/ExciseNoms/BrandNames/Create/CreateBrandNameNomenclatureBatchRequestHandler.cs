using AccountabilityInformationSystem.Api.Domain.Entities.Excise;
using AccountabilityInformationSystem.Api.Features.ExciseNoms.Shared.Create;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Constants;

namespace AccountabilityInformationSystem.Api.Features.ExciseNoms.BrandNames.Create;

public sealed class CreateBrandNameNomenclatureBatchRequestHandler(ApplicationDbContext dbContext)
    : CreateExciseNomenclatureBatchRequestHandler<BrandName, CreateBrandNameNomenclatureRequest>(dbContext)
{
    protected override string EntityIdPrefix => EntitiesConstants.BrandNameConstants.IdPrefix;
}
