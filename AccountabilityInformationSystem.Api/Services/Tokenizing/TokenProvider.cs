using System.Security.Claims;
using System.Text;
using AccountabilityInformationSystem.Api.Models.Identity.Auth;
using AccountabilityInformationSystem.Api.Settings;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;

namespace AccountabilityInformationSystem.Api.Services.Tokenizing;

public sealed class TokenProvider(IOptionsSnapshot<JwtAuthOptions> options)
{
    private readonly JwtAuthOptions _jwtAuthOptions = options.Value;

    public AccessTokenResponse Create(AccessTokenRequest request)
    {
        return new AccessTokenResponse(
            GenerateAccessToken(request),
            GenerateRefreshToken());
    }


    private string GenerateAccessToken(AccessTokenRequest tokenRequest)
    {
        SymmetricSecurityKey securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtAuthOptions.Key));
        SigningCredentials credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);

        List<Claim> claims =
        [
            new(JwtRegisteredClaimNames.Sub, tokenRequest.UserId),
            new(JwtRegisteredClaimNames.Email, tokenRequest.Email)
        ];

        SecurityTokenDescriptor tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(claims),
            Expires = DateTime.UtcNow.AddMinutes(_jwtAuthOptions.ExpirationInMinutes),
            SigningCredentials = credentials,
            Issuer = _jwtAuthOptions.Issuer,
            Audience = _jwtAuthOptions.Audience
        };

        JsonWebTokenHandler handler = new JsonWebTokenHandler();
        string accessToken = handler.CreateToken(tokenDescriptor);
        return accessToken;
    }

    private static string GenerateRefreshToken()
    {
        return string.Empty;
    }
}
