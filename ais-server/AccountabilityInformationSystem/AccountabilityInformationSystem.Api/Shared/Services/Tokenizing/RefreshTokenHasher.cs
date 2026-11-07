using System.Security.Cryptography;
using System.Text;

namespace AccountabilityInformationSystem.Api.Shared.Services.Tokenizing;

/// <summary>
/// Hashes refresh tokens before they are persisted or looked up.
/// The raw token is only ever sent to the client; the database stores the SHA-256 hash.
/// SHA-256 (not a slow KDF) is sufficient because refresh tokens are 64 random bytes,
/// and a deterministic hash is required for the unique-index lookup.
/// </summary>
public static class RefreshTokenHasher
{
    public static string Hash(string refreshToken)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(refreshToken);

        // Uppercase hex, no prefix — matches SQL Server CONVERT(varchar(64), HASHBYTES('SHA2_256', ...), 2)
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(refreshToken)));
    }
}
