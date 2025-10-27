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
    public const int EmailMaxLength = 128;
    public const int FirstNameMaxLength = 100;
    public const int MiddleNameMaxLength = 100;
    public const int LastNameMaxLength = 100;
    public const int ImageMaxLength = 2048;

    public const int CreatedByMaxLength = 128;
    public const int ModifiedByMaxLength = 128;
    public const int DeletedByMaxLength = 128;

    public const string DefaultSystemUser = "SystemUser";

    public const int TokenMaxlength = 1000;
}
