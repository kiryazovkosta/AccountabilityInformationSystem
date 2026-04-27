using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Shared;
using AccountabilityInformationSystem.Api.Features.Identity.Users.Shared;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace AccountabilityInformationSystem.Api.Features.Identity.Auth.Register;

public sealed class RegisterUserRequestHandler(
    ApplicationDbContext applicationDbContext,
    ApplicationIdentityDbContext identityDbContext,
    UserManager<IdentityUser> userManager,
    EmailConfirmationService emailConfirmationService)
{
    public async Task<Result> Handle(RegisterUserRequest request, CancellationToken cancellationToken)
    {
        Result? registerResult = 
            await identityDbContext.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
        {
            await using IDbContextTransaction transaction = await identityDbContext.Database.BeginTransactionAsync(cancellationToken);
            applicationDbContext.Database.SetDbConnection(identityDbContext.Database.GetDbConnection());
            await applicationDbContext.Database.UseTransactionAsync(transaction.GetDbTransaction(), cancellationToken);

            IdentityUser identityUser = new()
            {
                UserName = request.Username,
                Email = request.Email,
            };

            IdentityResult identityResult = await userManager.CreateAsync(identityUser, request.Password);
            if (!identityResult.Succeeded)
            {
                IReadOnlyList<Error> errors = [..identityResult.Errors
                    .Select(e => new Error(e.Code, e.Description))];
                return Result.Failure(errors, ResultFailureType.BadRequest);
            }

            identityResult = await userManager.AddToRoleAsync(identityUser, Role.Member);
            if (!identityResult.Succeeded)
            {
                IReadOnlyList<Error> errors = [..identityResult.Errors
                    .Select(e => new Error(e.Code, e.Description))] ;
                return Result.Failure(errors, ResultFailureType.BadRequest);
            }

            User user = request.ToEntity();
            user.IdentityId = identityUser.Id;

            await applicationDbContext.Users.AddAsync(user, cancellationToken);
            await applicationDbContext.SaveChangesAsync(cancellationToken);

            await identityDbContext.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            Result emailResult = await emailConfirmationService.SendConfirmationEmailAsync(identityUser, user);
            if (!emailResult.IsSuccess)
            {
                return emailResult;
            }

            return Result.Success(ResultSuccessType.Created);
        });

        return registerResult ?? Result.Failure(new Error("", "Unable to register user, please try again!"), ResultFailureType.BadRequest);
    }
}
