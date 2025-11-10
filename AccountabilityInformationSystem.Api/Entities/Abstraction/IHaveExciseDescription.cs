namespace AccountabilityInformationSystem.Api.Entities.Abstraction;

public interface IExciseEntity : IUsable
{
    string Code { get; }
    string BgDescription { get; }
    string? DescriptionEn { get; }
}
