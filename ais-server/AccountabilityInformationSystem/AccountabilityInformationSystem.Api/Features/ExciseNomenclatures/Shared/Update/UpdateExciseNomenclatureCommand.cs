using System.Diagnostics.CodeAnalysis;
using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;

namespace AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.Update;

[SuppressMessage("Major Code Smell", "S2326:Unused type parameters should be removed", Justification = "<Pending>")]
public record UpdateExciseNomenclatureCommand<TEntity>(string Id, UpdateExciseNomenclatureRequest Request)
    where TEntity : AuditableEntity, IEntity, IExciseEntity, new();
