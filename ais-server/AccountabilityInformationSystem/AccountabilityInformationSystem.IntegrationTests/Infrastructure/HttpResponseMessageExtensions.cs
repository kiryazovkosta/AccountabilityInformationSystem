using Microsoft.Net.Http.Headers;

namespace AccountabilityInformationSystem.IntegrationTests.Infrastructure;

public static class HttpResponseMessageExtensions
{
    public static string GetCookieValue(this HttpResponseMessage response, string name)
    {
        string prefix = $"{name}=";

        if (response.Headers.TryGetValues("Set-Cookie", out IEnumerable<string>? cookies))
        {
            string? cookie = cookies.FirstOrDefault(c => c.StartsWith(prefix, StringComparison.Ordinal));
            if (cookie is not null)
            {
                // ASP.NET Core URL-encodes cookie values (e.g. '+' in Base64 becomes %2B)
                return Uri.UnescapeDataString(cookie[prefix.Length..].Split(';', 2)[0]);
            }
        }

        throw new InvalidOperationException($"Response did not set the '{name}' cookie.");
    }

    public static bool IsCookieCleared(this HttpResponseMessage response, string name)
    {
        if (!response.Headers.TryGetValues("Set-Cookie", out IEnumerable<string>? cookies)
            || !SetCookieHeaderValue.TryParseList(cookies.ToList(), out IList<SetCookieHeaderValue>? parsed))
        {
            return false;
        }

        SetCookieHeaderValue? cookie = parsed.FirstOrDefault(c => c.Name.Equals(name, StringComparison.Ordinal));
        return cookie is not null
            && cookie.Value.Length == 0
            && cookie.Expires < DateTimeOffset.UtcNow;
    }

    // Builds the Cookie request header a browser would send back for the cookies this response set
    public static string ToCookieHeader(this HttpResponseMessage response)
        => string.Join("; ", response.Headers.GetValues("Set-Cookie").Select(c => c.Split(';', 2)[0]));
}
