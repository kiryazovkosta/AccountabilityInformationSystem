using AccountabilityInformationSystem.Api.Domain.Entities.Excise;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.Update;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Services.UserContexting;

namespace AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.CnCodes.Update;

public sealed class UpdateCnCodeNomenclatureRequestHandler(
    UserContext userContext,
    ApplicationDbContext dbContext)
    : UpdateExciseNomenclatureRequestHandler<CnCode>(userContext, dbContext);
