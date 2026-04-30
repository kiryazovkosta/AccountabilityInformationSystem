using AccountabilityInformationSystem.Api.Domain.Entities.Excise;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.ToggleStatus;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Services.UserContexting;

namespace AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.ApCodes.ToggleStatus;

public sealed class ToggleApCodeNomenclatureStatusRequestHandler(
    UserContext userContext,
    ApplicationDbContext dbContext)
    : ToggleExciseNomenclatureStatusRequestHandler<ApCode>(userContext, dbContext);
