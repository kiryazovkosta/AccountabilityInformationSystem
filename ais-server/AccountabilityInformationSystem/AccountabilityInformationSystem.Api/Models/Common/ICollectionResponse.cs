namespace AccountabilityInformationSystem.Api.Models.Common;

public interface ICollectionResponse<T>
{
    List<T> Items { get; init; }
}
