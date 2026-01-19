using System.ComponentModel;

namespace AccountabilityInformationSystem.Api.Domain.Entities.Flow;

public enum TransportType
{
    [Description("Авто")]
    Truck = 0,

    [Description("Железопътен")]
    Rail = 1,

    [Description("Морски")]
    Ship = 2,

    [Description("Въздушен")]
    Air = 3,

    [Description("Тръбопровод")]
    Pipeline = 4,

    [Description("Вътрешен Тръбопровод")]
    InnerPipeline = 5,

    [Description("Друг")]
    Other = 6
}
