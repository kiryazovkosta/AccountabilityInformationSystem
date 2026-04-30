using AccountabilityInformationSystem.Api.Domain.Entities.Excise;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.ToggleStatus;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Services.UserContexting;

namespace AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.BrandNames.ToggleStatus;

public sealed class ToggleBrandNameNomenclatureStatusRequestHandler(
    UserContext userContext,
    ApplicationDbContext dbContext)
    : ToggleExciseNomenclatureStatusRequestHandler<BrandName>(userContext, dbContext);
