using System.Linq.Dynamic.Core.Tokenizer;
using System.Net;
using System.Security;
using System.Security.Cryptography;
using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Login;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Refresh;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Register;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Shared;
using AccountabilityInformationSystem.Api.Features.Identity.Users.Shared;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Settings;
using AccountabilityInformationSystem.Api.Shared.Services.Tokenizing;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace AccountabilityInformationSystem.Api.Features.Identity.Auth;

[ApiController]
[Route("api/identity/auth")]
public sealed class AuthController(
    UserManager<IdentityUser> userManager,
    ApplicationIdentityDbContext identityDbContext,
    ApplicationDbContext applicationDbContext,
    TokenProvider tokenProvider,
    IAntiforgery antiforgery,
    IOptions<JwtAuthOptions> options) : ControllerBase
{
    private readonly JwtAuthOptions _jwtAuthOptions = options.Value;

    [HttpPost("register")]
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    public async Task<ActionResult> Register(
        RegisterUserRequest register,
        CancellationToken cancellationToken)
    {
        await identityDbContext.Database.CreateExecutionStrategy().ExecuteAsync(async () =>
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

            IdentityResult addToRoleResult = await userManager.AddToRoleAsync(identityUser, Role.Member);
            if (!addToRoleResult.Succeeded)
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

            User user = register.ToEntity();
            user.IdentityId = identityUser.Id;

            await applicationDbContext.Users.AddAsync(user, cancellationToken);

            await applicationDbContext.SaveChangesAsync(cancellationToken);

            AccessTokenResponse response = tokenProvider.Create(new AccessTokenRequest(identityUser.Id, identityUser.Email, [Role.Member]));

            RefreshToken refreshToken = new()
            {
                Id = $"rt_{Guid.CreateVersion7()}",
                UserId = identityUser.Id,
                Token = response.RefreshToken,
                ExpiresAt = DateTime.UtcNow.AddDays(_jwtAuthOptions.RefreshTokenExpirationDays)
            };

            await identityDbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);

            await identityDbContext.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);

            SetTokensInsideCookies(response, HttpContext);
            SetAntiforgeryToken();

            return Created();
        });

        return Problem(
            detail: "Unable to register user, please try again!",
            statusCode: StatusCodes.Status400BadRequest
        );
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Login(LoginUserRequest request, CancellationToken cancellationToken)
    {
        IdentityUser identityUser = await userManager.FindByEmailAsync(request.Email);
        if (identityUser is null ||
            !await userManager.CheckPasswordAsync(identityUser, request.Password))
        {
            return Problem(
                detail: "Invalid username or password!",
                statusCode: StatusCodes.Status401Unauthorized);
        }

        IEnumerable<string> roles = await userManager.GetRolesAsync(identityUser);

        AccessTokenRequest accessTokenRequest = new(identityUser.Id, identityUser.Email ?? string.Empty, roles);
        AccessTokenResponse response = tokenProvider.Create(accessTokenRequest);

        RefreshToken refreshToken = new()
        {
            Id = $"rt_{Guid.CreateVersion7()}",
            UserId = identityUser.Id,
            Token = response.RefreshToken,
            ExpiresAt = DateTime.UtcNow.AddDays(_jwtAuthOptions.RefreshTokenExpirationDays)
        };

        await identityDbContext.RefreshTokens.AddAsync(refreshToken, cancellationToken);

        await identityDbContext.SaveChangesAsync(cancellationToken);

        SetTokensInsideCookies(response, HttpContext);
        SetAntiforgeryToken();

        return Ok();
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    public async Task<ActionResult> Refresh(CancellationToken cancellationToken)
    {
        HttpContext.Request.Cookies.TryGetValue("refreshToken", out string? refreshTokenValue);
        if (!string.IsNullOrWhiteSpace(refreshTokenValue))
        {
            RefreshToken? refreshToken =
                await identityDbContext.RefreshTokens
                    .Include(rt => rt.User)
                    .FirstOrDefaultAsync(rt => rt.Token == refreshTokenValue, cancellationToken);
            if (refreshToken is null || refreshToken.ExpiresAt < DateTime.UtcNow)
            {
                return Problem(statusCode: StatusCodes.Status401Unauthorized, detail: "Unauthorized");
            }

            IEnumerable<string> roles = await userManager.GetRolesAsync(refreshToken.User);
            AccessTokenRequest accessTokenRequest = new(refreshToken.User.Id, refreshToken.User.Email ?? string.Empty, roles);
            AccessTokenResponse response = tokenProvider.Create(accessTokenRequest);

            refreshToken.Token = response.RefreshToken;
            refreshToken.ExpiresAt = DateTime.UtcNow.AddDays(_jwtAuthOptions.RefreshTokenExpirationDays);

            identityDbContext.RefreshTokens.Update(refreshToken);
            await identityDbContext.SaveChangesAsync(cancellationToken);

            SetTokensInsideCookies(response, HttpContext);
            SetAntiforgeryToken();

            return Ok();
        }

        return Problem(
            detail: "Unable to get refresh token, please try again!",
            statusCode: StatusCodes.Status401Unauthorized
        );
    }


    [HttpGet("csrf-token")]
    [AllowAnonymous]
    public IActionResult GetCsrfToken()
    {
        SetAntiforgeryToken();
        return NoContent();
    }

    [HttpPost("logout")]
    [Authorize]
    [IgnoreAntiforgeryToken]
    public async Task<ActionResult> Logout(CancellationToken cancellationToken)
    {
        if(HttpContext.Request.Cookies.TryGetValue("refreshToken", out string? refreshTokenValue))
        {
            RefreshToken? refreshToken = await identityDbContext.RefreshTokens.FirstOrDefaultAsync(rt => rt.Token == refreshTokenValue, cancellationToken);
            if (refreshToken is null || refreshToken.ExpiresAt < DateTime.UtcNow)
            {
                return Problem(statusCode: StatusCodes.Status401Unauthorized, detail: "Unauthorized");
            }

            identityDbContext.RefreshTokens.Remove(refreshToken);
            await identityDbContext.SaveChangesAsync(cancellationToken);
        }

        SetAccessTokenCookie("accessToken", string.Empty, -1440, HttpContext);
        SetAccessTokenCookie("refreshToken", string.Empty, -1440, HttpContext);
        SetAccessTokenCookie("XSRF-TOKEN", string.Empty, -1440, HttpContext);

        return Ok(new { message = "Successfully logged out!" });
    }

    private void SetTokensInsideCookies(AccessTokenResponse token, HttpContext context)
    {
        SetAccessTokenCookie("accessToken", token.AccessToken, 30, context);
        SetAccessTokenCookie("refreshToken", token.RefreshToken, 10080, context);
    }

    private static void SetAccessTokenCookie(string name, string value, int minutesExpires, HttpContext context)
    {
        context.Response.Cookies.Append(name, value,
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddMinutes(minutesExpires),
                HttpOnly = true,
                IsEssential = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/"
            });
    }

    private void SetAntiforgeryToken()
    {
        AntiforgeryTokenSet? tokens = antiforgery.GetAndStoreTokens(HttpContext);
        if (tokens is not null)
        {
            HttpContext.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken ?? string.Empty, new CookieOptions
            {
                HttpOnly = false,
                SameSite = SameSiteMode.None,
                Secure = true,
                IsEssential = true,
                Path = "/"
            });
        }
    }

}
