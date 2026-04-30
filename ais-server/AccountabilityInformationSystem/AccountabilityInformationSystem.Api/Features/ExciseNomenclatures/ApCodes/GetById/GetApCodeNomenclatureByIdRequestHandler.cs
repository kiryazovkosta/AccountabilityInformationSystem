using AccountabilityInformationSystem.Api.Domain.Entities.Excise;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.GetById;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Services.DataShaping;

namespace AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.ApCodes.GetById;

public sealed class GetApCodeNomenclatureByIdRequestHandler(
    DataShapingService dataShapingService,
    ApplicationDbContext dbContext) 
    : GetExciseNomenclatureByIdRequestHandler<ApCode>(dataShapingService, dbContext);
