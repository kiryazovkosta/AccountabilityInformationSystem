using AccountabilityInformationSystem.Api.Domain.Entities.Excise;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.ToggleStatus;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Services.UserContexting;

namespace AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.CnCodes.ToggleStatus;

public sealed class ToggleCnCodeNomenclatureStatusRequestHandler(
    UserContext userContext,
    ApplicationDbContext dbContext)
    : ToggleExciseNomenclatureStatusRequestHandler<CnCode>(userContext, dbContext);
