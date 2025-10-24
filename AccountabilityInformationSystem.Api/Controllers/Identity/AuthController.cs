using System.Net;
using AccountabilityInformationSystem.Api.Database;
using AccountabilityInformationSystem.Api.Entities.Identity;
using AccountabilityInformationSystem.Api.Models.Identity.Auth;
using AccountabilityInformationSystem.Api.Models.Identity.Users;
using AccountabilityInformationSystem.Api.Services.Tokenizing;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;

namespace AccountabilityInformationSystem.Api.Controllers.Identity;

[ApiController]
[Route("api/identity/auth")]
[AllowAnonymous]
public sealed class AuthController(
    UserManager<IdentityUser> userManager,
    ApplicationIdentityDbContext identityDbContext,
    ApplicationDbContext applicationDbContext,
    TokenProvider tokenProvider) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<AccessTokenResponse>> Register(
        RegisterUserRequest register, 
        CancellationToken cancellationToken)
    {
        await using IDbContextTransaction transaction = await identityDbContext.Database.BeginTransactionAsync(cancellationToken);
        applicationDbContext.Database.SetDbConnection(identityDbContext.Database.GetDbConnection());
        await applicationDbContext.Database.UseTransactionAsync(transaction.GetDbTransaction(), cancellationToken);

        IdentityUser identityUser = new()
        {
            UserName = register.Email,
            Email = register.Email,
        };

        IdentityResult identityResult = await userManager.CreateAsync(identityUser, register.Password);
        if (!identityResult.Succeeded)
        {
            Dictionary<string, object?> extensions = new()
            {
                { "errors", identityResult.Errors.ToDictionary(e => e.Code, e => e.Description) }
            };
            return Problem(
                detail: "Unable to register user, please try again!",
                statusCode: StatusCodes.Status400BadRequest, 
                extensions: extensions
                );
        }

        //await userManager.SetTwoFactorEnabledAsync(identityUser, true);

        User user = register.ToEntity();
        user.IdentityId = identityUser.Id;

        await applicationDbContext.Users.AddAsync(user, cancellationToken);

        await applicationDbContext.SaveChangesAsync(cancellationToken);

        await transaction.CommitAsync(cancellationToken);

        AccessTokenResponse response =
            tokenProvider.Create(new AccessTokenRequest(identityUser.Id, identityUser.Email));

        return Ok(response);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginUserRequest request)
    {
        IdentityUser user = await userManager.FindByEmailAsync(request.Email);
        if (user is null || 
            !await userManager.CheckPasswordAsync(user, request.Password))
        {
            return Problem(
                detail: "Invalid username or password!", 
                statusCode: StatusCodes.Status401Unauthorized);
        }

        AccessTokenRequest accessTokenRequest = new(user.Id, user.Email ?? string.Empty);
        AccessTokenResponse response = tokenProvider.Create(accessTokenRequest);

        return Ok(response);
    }
}
