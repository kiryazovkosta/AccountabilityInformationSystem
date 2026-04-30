using AccountabilityInformationSystem.Api.Domain.Entities.Excise;
using AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.GetAll;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Services.DataShaping;
using AccountabilityInformationSystem.Api.Shared.Services.Sorting;

namespace AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.BrandNames.GetAll;

public sealed class GetAllBrandNamesNomenclatureRequestHandler(
    SortMappingProvider sortMappingProvider,
    DataShapingService dataShapingService,
    ApplicationDbContext dbContext)
    : GetAllExciseNomenclaturesRequestHandler<BrandName>(sortMappingProvider, dataShapingService, dbContext);
