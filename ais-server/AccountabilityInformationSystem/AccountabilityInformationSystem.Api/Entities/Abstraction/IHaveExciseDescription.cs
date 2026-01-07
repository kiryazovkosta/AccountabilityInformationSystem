namespace AccountabilityInformationSystem.Api.Entities.Abstraction;

public interface IExciseEntity : IUsable
{
    string Code { get; set; }
    string BgDescription { get; set; }
    string? DescriptionEn { get; set; }
}
