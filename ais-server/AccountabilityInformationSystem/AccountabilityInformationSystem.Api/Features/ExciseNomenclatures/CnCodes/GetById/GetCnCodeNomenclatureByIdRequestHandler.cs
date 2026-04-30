using AccountabilityInformationSystem.Api.Domain.Entities.Excise;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.GetById;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Services.DataShaping;

namespace AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.CnCodes.GetById;

public sealed class GetCnCodeNomenclatureByIdRequestHandler(
    DataShapingService dataShapingService,
    ApplicationDbContext dbContext)
    : GetExciseNomenclatureByIdRequestHandler<CnCode>(dataShapingService, dbContext);
