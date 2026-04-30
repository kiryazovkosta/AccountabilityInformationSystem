using AccountabilityInformationSystem.Api.Domain.Entities.Excise;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.Update;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Services.UserContexting;

namespace AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.BrandNames.Update;

public sealed class UpdateBrandNameNomenclatureRequestHandler(
    UserContext userContext,
    ApplicationDbContext dbContext)
    : UpdateExciseNomenclatureRequestHandler<BrandName>(userContext, dbContext);
