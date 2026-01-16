using Microsoft.Build.Tasks;

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

    public const string DefaultSystemUser = "SystemUser";

    public const int TokenMaxlength = 1000;

    public const int ExciseDescriptionMaxLength = 1000;

    public static class ProductConstant
    {
        public const int CodeLength = 6;
        public const string CodePattern = @"^(?!000000)\d{6}$";
    }

    public static class ApCodeConstants
    {
        public const int CodeLength = 4;
        public const string CodePattern = @"^[A-Z]\d{3}$";
        public const int DescriptionMaxlength = 1000;
        public const string DefaultSorting = "Code ascending";
    }

    public static class BrandNameConstants
    {
        public const int CodeLength = 6;
        public const string CodePattern = @"^[A-Z]\d{5}$";
        public const int DescriptionMaxlength = 350;
        public const string DefaultSorting = "Code ascending";
    }

    public static class CnCodeConstants
    {
        public const int CodeLength = 8;
        public const string CodePattern = @"^(?!00000000)\d{8}$";
        public const int DescriptionMaxlength = 1000;
        public const string DefaultSorting = "Code ascending";
    }

}
