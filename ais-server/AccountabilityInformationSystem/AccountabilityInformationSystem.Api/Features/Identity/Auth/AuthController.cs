using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Register;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Login;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Refresh;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Shared;
using AccountabilityInformationSystem.Api.Features.Identity.Users.Shared;
using AccountabilityInformationSystem.Api.Shared.Services.Tokenizing;
using AccountabilityInformationSystem.Api.Settings;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using System.Net;
using System.Security.Cryptography;
using System.Security;

namespace AccountabilityInformationSystem.Api.Features.Identity.Auth;

[ApiController]
[Route("api/identity/auth")]
[AllowAnonymous]
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

            //await userManager.SetTwoFactorEnabledAsync(identityUser, true);

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

            this.SetTokensInsideCookies(response, HttpContext);
            SetAntiforgeryToken();

            return Created();
        });

        return Problem(
            detail: "Unable to register user, please try again!",
            statusCode: StatusCodes.Status400BadRequest
        );
    }

    [HttpPost("login")]
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

        this.SetTokensInsideCookies(response, HttpContext);
        SetAntiforgeryToken();

        return Ok();
    }

    [HttpPost("refresh")]
    [IgnoreAntiforgeryToken]
    public async Task<ActionResult> Refresh(CancellationToken cancellationToken)
    {
        HttpContext.Request.Cookies.TryGetValue("refreshToken", out var refreshTokenValue);
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

            this.SetTokensInsideCookies(response, HttpContext);
            SetAntiforgeryToken();

            return Ok();
        }

        return NotFound();
    }


    [HttpGet("csrf-token")]
    public IActionResult GetCsrfToken()
    {
        SetAntiforgeryToken();
        return NoContent();
    }

    private void SetTokensInsideCookies(AccessTokenResponse token, HttpContext context)
    {
        context.Response.Cookies.Append("accessToken", token.AccessToken,
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddMinutes(30),
                HttpOnly = true,
                IsEssential = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/"
            });

        context.Response.Cookies.Append("refreshToken", token.RefreshToken,
            new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(7),
                HttpOnly = true,
                IsEssential = true,
                Secure = true,
                SameSite = SameSiteMode.None,
                Path = "/"
            });
    }

    private void SetAntiforgeryToken()
    {
        var tokens = antiforgery.GetAndStoreTokens(HttpContext);
        HttpContext.Response.Cookies.Append("XSRF-TOKEN", tokens.RequestToken!, new CookieOptions
        {
            HttpOnly = false,
            SameSite = SameSiteMode.None,
            Secure = true,
            IsEssential = true,
            Path = "/"
        });
    }
}
