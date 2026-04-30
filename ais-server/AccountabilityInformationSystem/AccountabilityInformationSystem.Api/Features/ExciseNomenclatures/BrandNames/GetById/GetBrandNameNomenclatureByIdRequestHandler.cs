using AccountabilityInformationSystem.Api.Domain.Entities.Excise;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.GetById;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Services.DataShaping;

namespace AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.BrandNames.GetById;

public sealed class GetBrandNameNomenclatureByIdRequestHandler(
    DataShapingService dataShapingService,
    ApplicationDbContext dbContext)
    : GetExciseNomenclatureByIdRequestHandler<BrandName>(dataShapingService, dbContext);
