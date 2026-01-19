using System.ComponentModel;

namespace AccountabilityInformationSystem.Api.Domain.Entities.Flow;

public enum FlowDirectionType
{
    [Description("Вход")]
    Incoming = 0,
    [Description("Изход")]
    Outgoing = 1,
    [Description("Вход/Изход")]
    Both = 2
}
