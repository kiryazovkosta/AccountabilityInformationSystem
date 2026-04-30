using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Shared.Services.UserContexting;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Features.ExciseNomenclatures.Shared.Update;

public class UpdateExciseNomenclatureRequestHandler<TEntity>(
    UserContext userContext,
    ApplicationDbContext dbContext)
        where TEntity : AuditableEntity, IEntity, IExciseEntity, new()
{
    public async Task<Result> Handle(
        UpdateExciseNomenclatureCommand<TEntity> command,
        CancellationToken cancellationToken)
    {
        User? user = await userContext.GetUserAsync(cancellationToken);
        if (user is null)
        {
            return Result.Failure(
                new Error("User", "Unauthorized"),
                ResultFailureType.Unauthorized);
        }

        TEntity? entity = await dbContext
            .Set<TEntity>()
            .FirstOrDefaultAsync(en => en.Id == command.Id, cancellationToken);
        if (entity is null)
        {
            return Result.Failure(
                new Error("Id", $"{typeof(TEntity).Name} with specific id does not exist!"),
                ResultFailureType.NotFound);
        }

        bool codeExists = await dbContext
            .Set<TEntity>()
            .AnyAsync(en => en.Code == command.Request.Code && en.Id != entity.Id, cancellationToken);
        if (codeExists)
        {
            return Result.Failure(
                new Error("Code", $"{typeof(TEntity).Name} with specific code already exists!"),
                ResultFailureType.Conflict);
        }

        entity.UpdateFromRequest(command.Request, user.Email);
        await dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success(ResultSuccessType.NoContent);
    }
}
