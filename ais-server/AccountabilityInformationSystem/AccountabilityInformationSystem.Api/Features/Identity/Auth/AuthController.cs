using System.Diagnostics.CodeAnalysis;
using AccountabilityInformationSystem.Api.Domain.Entities.Abstraction;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.ChangePassword;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.ConfirmEmail;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.ForgotPassword;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Login;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Logout;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Refresh;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Register;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.ResendEmailConfirmation;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.ResetPassword;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.Shared;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.TwoFactor.NewDevice;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.TwoFactor.SetupTwoFactor;
using AccountabilityInformationSystem.Api.Features.Identity.Auth.TwoFactor.VerifyTwoFactor;
using AccountabilityInformationSystem.Api.Shared;
using AccountabilityInformationSystem.Api.Shared.Extensions;
using Microsoft.AspNetCore.Antiforgery;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Wolverine;

namespace AccountabilityInformationSystem.Api.Features.Identity.Auth;

[Route("api/identity/auth")]
[SuppressMessage(
    "Security",
    "S4502:Disabling CSRF protections is security-sensitive",
    Justification = "Pre-authentication endpoints and logout must work without an antiforgery token, which is only issued at login, refresh or GET csrf-token.")]
public sealed class AuthController(
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
    public async Task<IActionResult> Login(
        LoginUserRequest request,
        CancellationToken cancellationToken)
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
            SetCookies(result.Value!.AccessToken, result.Value.RefreshToken);
        }

        return result.ToActionResult();
    }

    [HttpGet("confirm-email", Name = "ConfirmEmailRoute")]
    [AllowAnonymous]
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
        SetupTwoFactorRequest request,
        CancellationToken cancellationToken)
    {
        Result<SetupTwoFactorResponse> result = await bus.InvokeAsync<Result<SetupTwoFactorResponse>>(request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("2fa/verify")]
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> VerifyTwoFactor(
        VerifyTwoFactorRequest request,
        CancellationToken cancellationToken)
    {
        Result<VerifyTwoFactorResponse> result = await bus.InvokeAsync<Result<VerifyTwoFactorResponse>>(request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("2fa/new-device")]
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> NewDevice(
        NewDeviceRequest request,
        CancellationToken cancellationToken)
    {
        Result<LoginUserResponse> result = await bus.InvokeAsync<Result<LoginUserResponse>>(request, cancellationToken);
        if (result.IsSuccessWith(ResultSuccessType.Ok) && result.Value is not null)
        {
            SetCookies(result.Value.AccessToken!, result.Value.RefreshToken!);
        }
        return result.ToActionResult();
    }

    [HttpPost("forgot-password")]
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> ForgotPassword(
        ForgotPasswordRequest request,
        CancellationToken cancellationToken)
    {
        Result result = await bus.InvokeAsync<Result>(request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("reset-password")]
    [AllowAnonymous]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> ResetPassword(
        ResetPasswordRequest request,
        CancellationToken cancellationToken)
    {
        Result result = await bus.InvokeAsync<Result>(request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpGet("resend-email-confirmation", Name = "ResendEmailConfirmationRoute")]
    [AllowAnonymous]
    public async Task<IActionResult> ResendEmailConfirmation(string email, CancellationToken cancellationToken)
    {
        ResendEmailConfirmationRequest request = new(email);
        Result result = await bus.InvokeAsync<Result>(request, cancellationToken);
        return result.ToActionResult();
    }

    [HttpPost("change-password")]
    [Authorize]
    public async Task<IActionResult> ChangePassword(
        ChangePasswordRequest request, 
        CancellationToken cancellationToken)
    {
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
    public async Task<IActionResult> Logout(CancellationToken cancellationToken)
    {
        HttpContext.Request.Cookies.TryGetValue("refreshToken", out string? refreshTokenValue);
        Result result = await bus.InvokeAsync<Result>(new LogoutRequest(refreshTokenValue), cancellationToken);
        if (result.IsFailure)
        {
            return result.ToActionResult();
        }

        SetAccessTokenCookie("accessToken", string.Empty, -1440, HttpContext);
        SetAccessTokenCookie("refreshToken", string.Empty, -1440, HttpContext);
        SetAccessTokenCookie("XSRF-TOKEN", string.Empty, -1440, HttpContext);

        return result.ToActionResult();
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

    [SuppressMessage(
        "Security",
        "S3330:Creating cookies without the \"HttpOnly\" flag is security-sensitive",
        Justification = "The Angular client reads XSRF-TOKEN and echoes it in the X-XSRF-TOKEN header, so it cannot be HttpOnly.")]
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
