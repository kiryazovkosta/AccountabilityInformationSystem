using AccountabilityInformationSystem.Api.Domain.Entities.Excise;
using AccountabilityInformationSystem.Api.Features.ExciseNoms.Shared.Update;
using AccountabilityInformationSystem.Api.Infrastructure.Data;

namespace AccountabilityInformationSystem.Api.Features.ExciseNoms.ApCodes.Update;

public sealed class UpdateApCodeNomenclatureRequestHandler(ApplicationDbContext dbContext)
    : UpdateExciseNomenclatureRequestHandler<ApCode, UpdateApCodeNomenclatureRequest>(dbContext);
