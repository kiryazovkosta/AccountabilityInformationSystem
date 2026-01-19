namespace AccountabilityInformationSystem.Api.Shared.Models;

public sealed record FieldsOnlyQueryParameters : AcceptHeaderQueryParameter
{
    public string? Fields { get; init; }
}
