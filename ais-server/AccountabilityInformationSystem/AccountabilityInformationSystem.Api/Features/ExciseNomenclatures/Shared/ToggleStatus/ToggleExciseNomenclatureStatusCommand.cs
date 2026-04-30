using System.Diagnostics.CodeAnalysis;
using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;

namespace AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.ToggleStatus;

[SuppressMessage("Major Code Smell", "S2326:Unused type parameters should be removed", Justification = "<Pending>")]
public record ToggleExciseNomenclatureStatusCommand<TEntity>(string Id)
    where TEntity : AuditableEntity, IEntity, IExciseEntity, new();
