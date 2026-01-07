using System.Reflection.Metadata;
using Microsoft.Identity.Client;

namespace AccountabilityInformationSystem.Api.Services.Sorting;

public sealed record SortMapping(string SortField, string PropertyName, bool Reverse = false);
