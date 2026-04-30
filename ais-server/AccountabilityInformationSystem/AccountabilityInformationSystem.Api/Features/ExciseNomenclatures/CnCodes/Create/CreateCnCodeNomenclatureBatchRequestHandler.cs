using AccountabilityInformationSystem.Api.Domain.Entities.Excise;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.Create;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Services.UserContexting;

namespace AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.CnCodes.Create;

public sealed class CreateCnCodeNomenclatureBatchRequestHandler(
    UserContext userContext,
    ApplicationDbContext dbContext)
    : CreateExciseNomenclatureBatchRequestHandler<CnCode, CreateCnCodeNomenclatureRequest>(userContext, dbContext);
