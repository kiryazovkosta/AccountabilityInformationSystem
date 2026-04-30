using AccountabilityInformationSystem.Api.Domain.Entities.Excise;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.Create;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Services.UserContexting;

namespace AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.BrandNames.Create;

public sealed class CreateBrandNameNomenclatureBatchRequestHandler(
    UserContext userContext,
    ApplicationDbContext dbContext)
    : CreateExciseNomenclatureBatchRequestHandler<BrandName, CreateBrandNameNomenclatureRequest>(userContext, dbContext);
