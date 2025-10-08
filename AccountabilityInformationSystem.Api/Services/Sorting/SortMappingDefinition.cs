namespace AccountabilityInformationSystem.Api.Services.Sorting;

public sealed class SortMappingDefinition<TSource, TDestination> : ISortMappingDefinition
    where TSource: class
    where TDestination: class
{
    public required SortMapping[] Mappings { get; init; }

    // Fix S2326: Use TSource and TDestination in the class
    public Type SourceType => typeof(TSource);
    public Type DestinationType => typeof(TDestination);
}
