using System.ComponentModel;
using System.Reflection;

namespace AccountabilityInformationSystem.Api.Shared.Extensions;

public static class EnumExtensions
{
    public static string GetDescription(this Enum value)
    {
        FieldInfo? fieldInfo = value.GetType().GetField(value.ToString());
        if (fieldInfo is null)
        {
            throw new ArgumentException("Invalid enum value", nameof(value));
        }

        if (fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false).FirstOrDefault()
            is not DescriptionAttribute descriptionAttribute)
        {
            throw new InvalidOperationException($"Enum value '{value}' is missing a Description attribute.");
        }

        return descriptionAttribute.Description;
    }
}
