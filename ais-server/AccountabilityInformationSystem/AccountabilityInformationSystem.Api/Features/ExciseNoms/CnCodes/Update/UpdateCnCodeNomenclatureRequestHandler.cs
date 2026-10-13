using AccountabilityInformationSystem.Api.Domain.Entities.Excise;
using AccountabilityInformationSystem.Api.Features.ExciseNoms.Shared.Update;
using AccountabilityInformationSystem.Api.Infrastructure.Data;

namespace AccountabilityInformationSystem.Api.Features.ExciseNoms.CnCodes.Update;

public sealed class UpdateCnCodeNomenclatureRequestHandler(ApplicationDbContext dbContext)
    : UpdateExciseNomenclatureRequestHandler<CnCode, UpdateCnCodeNomenclatureRequest>(dbContext);
