using AccountabilityInformationSystem.Api.Domain.Entities.Excise;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.GetAll;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Services.DataShaping;
using AccountabilityInformationSystem.Api.Shared.Services.Sorting;

namespace AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.CnCodes.GetAll;

public sealed class GetAllCnCodesNomenclatureRequestHandler(
    SortMappingProvider sortMappingProvider,
    DataShapingService dataShapingService,
    ApplicationDbContext dbContext)
    : GetAllExciseNomenclaturesRequestHandler<CnCode>(sortMappingProvider, dataShapingService, dbContext);
