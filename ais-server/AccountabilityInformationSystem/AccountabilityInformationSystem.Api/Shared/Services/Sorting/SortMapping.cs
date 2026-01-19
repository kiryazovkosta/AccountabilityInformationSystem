namespace AccountabilityInformationSystem.Api.Shared.Services.Sorting;

public sealed record SortMapping(string SortField, string PropertyName, bool Reverse = false);
