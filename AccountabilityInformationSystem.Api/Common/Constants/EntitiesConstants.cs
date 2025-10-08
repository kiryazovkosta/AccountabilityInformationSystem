namespace AccountabilityInformationSystem.Api.Common.Constants;

public static class EntitiesConstants
{
    public const int IdMaxLength = 50;
    public const int ExciseNumberLength = 13;
    public const string ExciseNumberPattern = @"^BGNCA\d{8}$";
    public const int NameMaxLength = 200;
    public const int FullNameMaxLength = 500;
    public const int DescriptionMaxLength = 2000;
    public const int ControlPointMaxLength = 19;
    public const string ControlPointPattern = @"^BGNCA\d{14}$";
    public const int EnumMaxLength = 50;
    public const int OrderPositionMinValue = 0;

    public const int CreatedByMaxLength = 128;
    public const int ModifiedByMaxLength = 128;
    public const int DeletedByMaxLength = 128;
}
