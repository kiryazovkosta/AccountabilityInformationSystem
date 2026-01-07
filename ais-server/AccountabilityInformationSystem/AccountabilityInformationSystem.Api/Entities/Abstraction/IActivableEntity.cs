namespace AccountabilityInformationSystem.Api.Entities.Abstraction;

public interface IActivableEntity
{
    DateOnly ActiveFrom { get; set; }
    DateOnly ActiveTo { get; set; }
}
