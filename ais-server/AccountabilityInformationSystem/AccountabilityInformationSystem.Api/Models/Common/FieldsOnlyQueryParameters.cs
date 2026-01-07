namespace AccountabilityInformationSystem.Api.Models.Common;

public sealed record FieldsOnlyQueryParameters : AcceptHeaderQueryParameter
{
    public string? Fields { get; init; }
}
