using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Shared;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Features.Identity.Auth.ResendEmailConfirmation;

public sealed class ResendEmailConfirmationRequestHandler(
    ApplicationDbContext applicationDbContext,
    UserManager<IdentityUser> userManager,
    EmailConfirmationService emailConfirmationService)
{
    public async Task<Result> Handle(ResendEmailConfirmationRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Email))
        {
            return Result.Failure(new Error("email", "Email cannot be null or empty string!"));
        }

        IdentityUser? identityUser = await userManager.FindByEmailAsync(request.Email);
        if (identityUser is null)
        {
            return Result.Failure(new Error("email", "User with provided email does not exists!"), ResultFailureType.NotFound);
        }

        User? user = await applicationDbContext.Users.FirstOrDefaultAsync(u => u.IdentityId == identityUser.Id, cancellationToken);
        if (user is null)
        {
            return Result.Failure(new Error("email", "User with provided email does not exists!"), ResultFailureType.NotFound);
        }

        if (identityUser.EmailConfirmed)
        {
            return Result.Failure(new Error("email", "Email is already confirmed!"));
        }

        return await emailConfirmationService.SendConfirmationEmailAsync(identityUser, user);
    }
}
