using System.Diagnostics.CodeAnalysis;
using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Shared.Models;

namespace AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.GetAll;

[SuppressMessage("Major Code Smell", "S2326:Unused type parameters should be removed", Justification = "<Pending>")]
public record GetAllExciseNomenclaturesRequest<TEntity>(ExciseNomenclatureQueryParameters Query)
    where TEntity : AuditableEntity, IEntity, IExciseEntity, new();
