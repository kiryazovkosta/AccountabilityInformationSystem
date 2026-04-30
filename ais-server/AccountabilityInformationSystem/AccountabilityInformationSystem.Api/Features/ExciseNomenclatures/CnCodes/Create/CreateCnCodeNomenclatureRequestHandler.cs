using AccountabilityInformationSystem.Api.Domain.Entities.Excise;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.Create;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Services.UserContexting;

namespace AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.CnCodes.Create;

public sealed class CreateCnCodeNomenclatureRequestHandler(
    UserContext userContext,
    ApplicationDbContext dbContext)
    : CreateExciseNomenclatureRequestHandler<CnCode, CreateCnCodeNomenclatureRequest>(userContext, dbContext);
