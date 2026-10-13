using AccountabilityInformationSystem.Api.Domain.Entities.Excise;
using AccountabilityInformationSystem.Api.Features.ExciseNoms.Shared.ToggleStatus;
using AccountabilityInformationSystem.Api.Infrastructure.Data;

namespace AccountabilityInformationSystem.Api.Features.ExciseNoms.BrandNames.ToggleStatus;

public sealed class ToggleBrandNameNomenclatureStatusRequestHandler(ApplicationDbContext dbContext)
    : ToggleExciseNomenclatureStatusRequestHandler<BrandName>(dbContext);
