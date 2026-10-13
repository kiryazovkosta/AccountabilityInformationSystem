using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Features.ExciseNoms.Shared.Create;

public abstract class CreateExciseNomenclatureRequestHandler<TEntity, TCreateRequest>(
    ApplicationDbContext dbContext)
    where TEntity : AuditableEntity, IEntity, IExciseEntity, new()
    where TCreateRequest : CreateExciseNomenclatureRequest
{
    protected abstract string EntityIdPrefix { get; }

    public async Task<Result<ExciseNomenclatureResponse>> Handle(
        TCreateRequest request,
        CancellationToken cancellationToken)
    {
        bool codeExists = await dbContext
            .Set<TEntity>()
            .AnyAsync(en => en.Code == request.Code, cancellationToken);
        if (codeExists)
        {
            return Result<ExciseNomenclatureResponse>.Failure(
                new Error("Code", $"{typeof(TEntity).Name} with specific code already exists!"),
                ResultFailureType.Conflict);
        }

        TEntity entity = request.ToEntity<TEntity>(EntityIdPrefix);
        await dbContext.Set<TEntity>().AddAsync(entity, cancellationToken);
        await dbContext.SaveChangesAsync(cancellationToken);

        return Result<ExciseNomenclatureResponse>.Success(entity.ToResponse());
    }
}
