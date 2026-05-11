using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Shared;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace AccountabilityInformationSystem.Api.Features.Identity.Auth.ForgotPassword;

public sealed class ForgotPasswordRequestHandler(
    ApplicationDbContext applicationDbContext,
    UserManager<IdentityUser> userManager,
    PasswordResetEmailService passwordResetEmailService)
{
    public async Task<Result> Handle(ForgotPasswordRequest request, CancellationToken cancellationToken)
    {
        IdentityUser? identityUser = await userManager.FindByNameAsync(request.Username);
        if (identityUser is null)
        {
            return Result.Success(ResultSuccessType.NoContent);
        }

        User? user = await applicationDbContext.Users
            .FirstOrDefaultAsync(u => u.IdentityId == identityUser.Id, cancellationToken);
        if (user is null)
        {
            return Result.Success(ResultSuccessType.NoContent);
        }

        Result emailResult = await passwordResetEmailService.SendResetEmailAsync(identityUser, user);
        if (!emailResult.IsSuccess)
        {
            return emailResult;
        }

        return Result.Success(ResultSuccessType.NoContent);
    }
}
