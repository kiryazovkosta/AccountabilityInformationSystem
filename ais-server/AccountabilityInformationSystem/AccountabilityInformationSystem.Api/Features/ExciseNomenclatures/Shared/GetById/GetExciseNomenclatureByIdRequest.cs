using System.Diagnostics.CodeAnalysis;
using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Shared.Models;

namespace AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.GetById;

[SuppressMessage("Major Code Smell", "S2326:Unused type parameters should be removed", Justification = "<Pending>")]
public record GetExciseNomenclatureByIdRequest<TEntity>(string Id, FieldsOnlyQueryParameters Query)
    where TEntity : AuditableEntity, IEntity, IExciseEntity, new();
