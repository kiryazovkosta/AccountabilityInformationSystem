using System.Reflection;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using NetArchTest.Rules;
using ArchTestResult = NetArchTest.Rules.TestResult;

namespace AccountabilityInformationSystem.UnitTests.Architecture;

// Enforces the vertical slice dependency rules on the Api assembly.
// A feature is a namespace under Features that contains a concrete controller.
// A slice is a namespace under a feature that contains a concrete *Handler, outside any Shared folder.
// NetArchTest matches dependency names on whole namespace segments, so "GetAll" does not also match "GetAllV2".
public sealed class VerticalSliceArchitectureTests
{
    private const string ApiNamespace = "AccountabilityInformationSystem.Api";
    private const string FeaturesNamespace = $"{ApiNamespace}.Features";

    private static readonly Assembly ApiAssembly = typeof(Program).Assembly;

    private static readonly string[] FeatureNamespaces = [.. ApiAssembly.GetTypes()
        .Where(type => type is { IsAbstract: false, Namespace: not null }
            && IsWithin(type.Namespace, FeaturesNamespace)
            && typeof(ControllerBase).IsAssignableFrom(type))
        .Select(type => type.Namespace!)
        .Distinct()
        .Order()];

    private static readonly string[] SliceNamespaces = [.. ApiAssembly.GetTypes()
        .Where(type => type is { IsAbstract: false, IsGenericTypeDefinition: false, Namespace: not null }
            && type.Name.EndsWith("Handler", StringComparison.Ordinal)
            && !type.Namespace.Split('.').Contains("Shared"))
        .Select(type => type.Namespace!)
        .Where(@namespace => FeatureOf(@namespace) is not null)
        .Distinct()
        .Order()];

    public static TheoryData<string> NonFeatureNamespaces => ToTheoryData(
    [
        $"{ApiNamespace}.Shared",
        $"{ApiNamespace}.Infrastructure",
        $"{ApiNamespace}.Domain",
        $"{ApiNamespace}.Middleware",
        $"{ApiNamespace}.Interceptors",
        $"{ApiNamespace}.Settings"
    ]);

    public static TheoryData<string> Features => ToTheoryData(FeatureNamespaces);

    public static TheoryData<string> Slices => ToTheoryData(SliceNamespaces);

    [Fact]
    public void Conventions_ShouldDiscoverFeaturesAndSlices()
    {
        Assert.Contains($"{FeaturesNamespace}.Warehouses", FeatureNamespaces);
        Assert.Contains($"{FeaturesNamespace}.Identity.Auth", FeatureNamespaces);
        Assert.Contains($"{FeaturesNamespace}.Warehouses.Create", SliceNamespaces);
        Assert.Contains($"{FeaturesNamespace}.Identity.Auth.TwoFactor.NewDevice", SliceNamespaces);
    }

    [Theory]
    [MemberData(nameof(NonFeatureNamespaces))]
    public void NonFeatureCode_ShouldNotDependOnFeatures(string @namespace)
    {
        ArchTestResult result = Types.InAssembly(ApiAssembly)
            .That().ResideInNamespaceMatching(Subtree(@namespace))
            .ShouldNot().HaveDependencyOn(FeaturesNamespace)
            .GetResult();

        AssertSuccessful(result);
    }

    [Theory]
    [MemberData(nameof(Features))]
    public void Feature_ShouldNotDependOnOtherFeatures(string feature)
    {
        string[] otherFeatures = [.. FeatureNamespaces
            .Where(other => !IsWithin(other, feature) && !IsWithin(feature, other))];

        ArchTestResult result = Types.InAssembly(ApiAssembly)
            .That().ResideInNamespaceMatching(Subtree(feature))
            .ShouldNot().HaveDependencyOnAny(otherFeatures)
            .GetResult();

        AssertSuccessful(result);
    }

    [Theory]
    [MemberData(nameof(Slices))]
    public void Slice_ShouldNotDependOnSiblingSlices(string slice)
    {
        string? feature = FeatureOf(slice);
        string[] siblingSlices = [.. SliceNamespaces
            .Where(other => other != slice && FeatureOf(other) == feature)];
        if (siblingSlices.Length == 0)
        {
            return;
        }

        ArchTestResult result = Types.InAssembly(ApiAssembly)
            .That().ResideInNamespaceMatching(Subtree(slice))
            .ShouldNot().HaveDependencyOnAny(siblingSlices)
            .GetResult();

        AssertSuccessful(result);
    }

    [Theory]
    [MemberData(nameof(Features))]
    public void FeatureShared_ShouldNotDependOnItsSlices(string feature)
    {
        string[] slices = [.. SliceNamespaces
            .Where(slice => FeatureOf(slice) == feature)];
        if (slices.Length == 0)
        {
            return;
        }

        ArchTestResult result = Types.InAssembly(ApiAssembly)
            .That().ResideInNamespaceMatching(Subtree($"{feature}.Shared"))
            .ShouldNot().HaveDependencyOnAny(slices)
            .GetResult();

        AssertSuccessful(result);
    }

    private static string? FeatureOf(string @namespace) => FeatureNamespaces
        .Where(feature => IsWithin(@namespace, feature))
        .MaxBy(feature => feature.Length);

    private static bool IsWithin(string @namespace, string ancestor) =>
        @namespace == ancestor || @namespace.StartsWith($"{ancestor}.", StringComparison.Ordinal);

    private static string Subtree(string @namespace) => $@"^{Regex.Escape(@namespace)}(\..+)?$";

    private static void AssertSuccessful(ArchTestResult result) =>
        Assert.True(
            result.IsSuccessful,
            $"Violating types: {string.Join(", ", result.FailingTypeNames ?? [])}");

    private static TheoryData<string> ToTheoryData(IEnumerable<string> values)
    {
        TheoryData<string> data = new();
        foreach (string value in values)
        {
            data.Add(value);
        }

        return data;
    }
}
