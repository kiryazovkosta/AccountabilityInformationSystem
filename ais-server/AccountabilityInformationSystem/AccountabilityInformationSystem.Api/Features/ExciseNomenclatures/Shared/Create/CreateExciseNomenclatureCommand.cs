using System.Diagnostics.CodeAnalysis;
using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;

namespace AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.Create;

[SuppressMessage("Major Code Smell", "S2326:Unused type parameters should be removed", Justification = "<Pending>")]
public record CreateExciseNomenclatureCommand<TEntity, TCreateRequest>(
    TCreateRequest Request,
    string EntityIdPrefix)
    where TEntity : AuditableEntity, IEntity, IExciseEntity, new()
    where TCreateRequest : CreateExciseNomenclatureRequest;

[SuppressMessage("Major Code Smell", "S2326:Unused type parameters should be removed", Justification = "<Pending>")]
public record CreateExciseNomenclatureBatchCommand<TEntity, TCreateRequest>(
    List<TCreateRequest> Entries,
    string EntityIdPrefix)
    where TEntity : AuditableEntity, IEntity, IExciseEntity, new()
    where TCreateRequest : CreateExciseNomenclatureRequest;
