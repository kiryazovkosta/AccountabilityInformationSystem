namespace AccountabilityInformationSystem.Api.Shared.Models;

public interface ICollectionResponse<T>
{
    List<T> Items { get; init; }
}
