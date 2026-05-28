using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Login;
using AccountabilityInformationSystem.Api.Shared.Services.UserContexting;
using Microsoft.AspNetCore.Identity;

namespace AccountabilityInformationSystem.Api.Features.Identity.Auth.ChangePassword;

public sealed class ChangePasswordRequestHandler(
    UserContext userContext,
    UserManager<IdentityUser> userManager)
{
    public async Task<Result> Handle(ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        User? user = await userContext.GetUserAsync(cancellationToken);
        if (user is null || string.IsNullOrEmpty(user.IdentityId))
        {
            return Result.Failure(new Error("user", "Unauthorized request"), ResultFailureType.Unauthorized);
        }

        IdentityUser? identityUser = await userManager.FindByIdAsync(user.IdentityId);
        if (identityUser is null)
        {
            return Result.Failure(new Error("user", "User not found"), ResultFailureType.NotFound);
        }

        if (identityUser.TwoFactorEnabled)
        {
            bool isValid = await userManager.VerifyTwoFactorTokenAsync(
                identityUser,
                userManager.Options.Tokens.AuthenticatorTokenProvider,
                request.Code ?? string.Empty);

            if (!isValid)
            {
                return Result.Failure(new Error("user", "2FA code is not provided or is invalid!"), ResultFailureType.Unauthorized);
            }
        }

        IdentityResult? identityResult = await userManager.ChangePasswordAsync(
            identityUser,
            request.OldPassword,
            request.NewPassword);
        if (!identityResult.Succeeded)
        {
            IReadOnlyList<Error> errors = [..identityResult.Errors
                    .Select(e => new Error(e.Code, e.Description))];
            return Result.Failure(errors, ResultFailureType.BadRequest);
        }

        return Result.Success(ResultSuccessType.NoContent);
    }
}
