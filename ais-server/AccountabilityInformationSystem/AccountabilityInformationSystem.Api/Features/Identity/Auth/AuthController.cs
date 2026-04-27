using System.Linq.Dynamic.Core.Tokenizer;
using System.Net;
using System.Security;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Domain.Entities.Identity;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.ConfirmEmail;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Login;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Refresh;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Register;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.ResendEmailConfirmation;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Shared;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.TwoFactor.SetupTwoFactor;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.TwoFactor.VerifyTwoFactor;
using AccountabilityInformationSystem.Api.Features.Identity.Users.Shared;
using AccountabilityInformationSystem.Api.Infrastructure.Data;
using AccountabilityInformationSystem.Api.Settings;
using AccountabilityInformationSystem.Api.Shared;
using AccountabilityInformationSystem.Api.Shared.Extensions;
using AccountabilityInformationSystem.Api.Shared.Services.Tokenizing;
using Mapster;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using QRCoder;
using Wolverine;

namespace AccountabilityInformationSystem.Api.Features.Identity.Auth;

[Route("api/identity/auth")]
public sealed class AuthController(
    ApplicationIdentityDbContext identityDbContext,
    IAntiforgery antiforgery,
    IMessageBus bus) : ApiController
{

    [HttpPost("register")]
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Register(
        RegisterUserRequest registerRequest,
        CancellationToken cancellationToken)
    {
        Result result = await bus.InvokeAsync<Result>(registerRequest, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("login")]
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Login(LoginUserRequest request, CancellationToken cancellationToken)
    {
        Result<LoginUserResponse> result = await bus.InvokeAsync<Result<LoginUserResponse>>(request, cancellationToken);
        if (result.IsSuccessWith(ResultSuccessType.Ok) && result.Value is not null)
        {
            SetCookies(result.Value.AccessToken!, result.Value.RefreshToken!);
        }

        return result.ToActionResult();
    }

    [HttpPost("refresh")]
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Refresh(CancellationToken cancellationToken)
    {
        if (!HttpContext.Request.Cookies.TryGetValue("refreshToken", out string? refreshTokenValue) 
            || string.IsNullOrWhiteSpace(refreshTokenValue))
        {
            return Result.Failure(
                new Error("Unauthorized", "Refresh token missing."),
                ResultFailureType.Unauthorized)
                .ToActionResult();
        }

        RefreshTokenRequest request = new() { RefreshToken = refreshTokenValue };
        Result<AccessTokenResponse> result = await bus.InvokeAsync<Result<AccessTokenResponse>>(request, cancellationToken);
        if (result.IsSuccessWith(ResultSuccessType.Ok))
        {
            SetCookies(result.Value!.AccessToken!, result.Value.RefreshToken!);
        }

        return result.ToActionResult();
    }

    [HttpGet("confirm-email", Name = "ConfirmEmailRoute")]
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> ConfirmEmail(string userId, string code, CancellationToken cancellationToken)
    {
        ConfirmEmailRequest request = new(userId, code);
        Result<ConfirmEmailResponse> result = await bus.InvokeAsync<Result<ConfirmEmailResponse>>(request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("2fa/setup")]
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> SetupTwoFactor(
        SetupTwoFactorRequest request)
    {
        Result<SetupTwoFactorResponse> result = await bus.InvokeAsync<Result<SetupTwoFactorResponse>>(request);
        return result.ToActionResult();
    }

    [HttpPost("2fa/verify")]
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> VerifyTwoFactor(
        VerifyTwoFactorRequest request)
    {
        Result<VerifyTwoFactorResponse> result = await bus.InvokeAsync<Result<VerifyTwoFactorResponse>>(request);
        return result.ToActionResult() ;
    }

    [HttpGet("resend-email-confirmation", Name = "ResendEmailConfirmationRoute")]
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> ResendEmailConfirmation(string email, CancellationToken cancellationToken)
    {
        ResendEmailConfirmationRequest request = new(email);
        Result result = await bus.InvokeAsync<Result>(request, cancellationToken);
        return result.ToActionResult();
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

    private void SetCookies(string accessToken, string refreshToken)
    {
        SetTokensInsideCookies(new AccessTokenResponse(accessToken, refreshToken), HttpContext);
        SetAntiforgeryToken();
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
