
namespace AccountabilityInformationSystem.Api.Services.Sorting;

public sealed class SortMappingProvider(IEnumerable<ISortMappingDefinition> sortMappingDefinitions)
{
    public SortMapping[] GetMappings<TSource, TDestination>()
        where TSource : class
        where TDestination : class
    {
        SortMappingDefinition<TSource, TDestination>? sortMappingDefinition = sortMappingDefinitions
            .OfType<SortMappingDefinition<TSource, TDestination>>()
            .FirstOrDefault();
        if (sortMappingDefinition is null)
        {
            throw new InvalidOperationException($"The mapping definition from '{typeof(TSource).Name}' to '{typeof(TDestination).Name}' was not found.");
        }

        return sortMappingDefinition?.Mappings;
    }

    public bool ValidateMappings<TSource, TDestination>(string? sort)
        where TSource : class
        where TDestination : class
    {
        if (string.IsNullOrWhiteSpace(sort))
        {
            return true;
        }

        List<string> sortFields = sort
            .Split(',')
            .Select(s => s.Trim().Split(' ')[0])
            .Where(s => !string.IsNullOrWhiteSpace(s))
            .ToList();

        SortMapping[] mappings = GetMappings<TSource, TDestination>();

        return sortFields.All(field => mappings.Any(m => 
            m.SortField.Equals(field, StringComparison.OrdinalIgnoreCase)));
    }
}
