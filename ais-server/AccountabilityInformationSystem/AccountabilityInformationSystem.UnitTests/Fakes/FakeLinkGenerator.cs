using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

namespace AccountabilityInformationSystem.UnitTests.Fakes;

// Hand-rolled fake for LinkGenerator - no mocking library is available in this solution, so this subclasses
// the real abstract LinkGenerator and implements its members with a deterministic canned URI built from the
// route values passed in, which is enough to assert on link shape (action name, query values) without a real
// ASP.NET Core routing host. Only GetUriByAddress(HttpContext, ...) is exercised by LinkService.Create, since
// LinkGenerator.GetUriByAction ultimately calls that overload with a RouteValuesAddress.
public sealed class FakeLinkGenerator : LinkGenerator
{
    public override string? GetPathByAddress<TAddress>(
        HttpContext httpContext,
        TAddress address,
        RouteValueDictionary values,
        RouteValueDictionary? ambientValues = null,
        PathString? pathBase = null,
        FragmentString fragment = default,
        LinkOptions? options = null) =>
        throw new NotSupportedException();

    public override string? GetPathByAddress<TAddress>(
        TAddress address,
        RouteValueDictionary values,
        PathString pathBase = default,
        FragmentString fragment = default,
        LinkOptions? options = null) =>
        throw new NotSupportedException();

    public override string? GetUriByAddress<TAddress>(
        HttpContext httpContext,
        TAddress address,
        RouteValueDictionary values,
        RouteValueDictionary? ambientValues = null,
        string? scheme = null,
        HostString? host = null,
        PathString? pathBase = null,
        FragmentString fragment = default,
        LinkOptions? options = null)
    {
        if (address is not RouteValuesAddress routeValuesAddress)
        {
            throw new NotSupportedException();
        }

        string action = routeValuesAddress.ExplicitValues["action"]?.ToString() ?? string.Empty;
        string query = string.Join(
            '&',
            routeValuesAddress.ExplicitValues
                .Where(kv => kv.Key is not ("action" or "controller") && kv.Value is not null)
                .Select(kv => $"{kv.Key}={kv.Value}"));

        return query.Length == 0 ? $"https://fake.test/{action}" : $"https://fake.test/{action}?{query}";
    }

    public override string? GetUriByAddress<TAddress>(
        TAddress address,
        RouteValueDictionary values,
        string scheme,
        HostString host,
        PathString pathBase = default,
        FragmentString fragment = default,
        LinkOptions? options = null) =>
        throw new NotSupportedException();
}
