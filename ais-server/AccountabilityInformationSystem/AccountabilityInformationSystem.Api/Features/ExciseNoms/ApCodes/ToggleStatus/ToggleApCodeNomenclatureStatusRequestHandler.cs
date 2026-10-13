using AccountabilityInformationSystem.Api.Domain.Entities.Excise;
using AccountabilityInformationSystem.Api.Features.ExciseNoms.Shared.ToggleStatus;
using AccountabilityInformationSystem.Api.Infrastructure.Data;

namespace AccountabilityInformationSystem.Api.Features.ExciseNoms.ApCodes.ToggleStatus;

public sealed class ToggleApCodeNomenclatureStatusRequestHandler(ApplicationDbContext dbContext)
    : ToggleExciseNomenclatureStatusRequestHandler<ApCode>(dbContext);
