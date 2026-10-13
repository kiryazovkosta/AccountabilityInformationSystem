using AccountabilityInformationSystem.Api.Domain.Entities.Excise;
using AccountabilityInformationSystem.Api.Features.ExciseNoms.Shared.ToggleStatus;
using AccountabilityInformationSystem.Api.Infrastructure.Data;

namespace AccountabilityInformationSystem.Api.Features.ExciseNoms.CnCodes.ToggleStatus;

public sealed class ToggleCnCodeNomenclatureStatusRequestHandler(ApplicationDbContext dbContext)
    : ToggleExciseNomenclatureStatusRequestHandler<CnCode>(dbContext);
