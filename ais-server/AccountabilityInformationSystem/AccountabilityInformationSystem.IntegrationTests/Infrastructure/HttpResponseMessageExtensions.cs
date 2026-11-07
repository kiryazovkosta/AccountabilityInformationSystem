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
}
