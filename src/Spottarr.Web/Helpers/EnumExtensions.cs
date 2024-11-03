using System.ComponentModel.DataAnnotations;
using System.Reflection;

namespace Spottarr.Web.Helpers;

internal static class EnumExtensions
{
    public static T? GetAttributeOfType<T>(this Enum enumValue) where T : Attribute
    {
        var type = enumValue.GetType();
        var memInfo = type.GetField(enumValue.ToString(), BindingFlags.Public | BindingFlags.Static);
        var attributes = memInfo?.GetCustomAttributes<T>(false);
        return attributes?.FirstOrDefault();
    }

    public static string GetDisplayName(this Enum enumValue)
    {
        var attribute = enumValue.GetAttributeOfType<DisplayAttribute>();
        return (attribute == null ? enumValue.ToString() : attribute.Name) ?? string.Empty;
    }

    public static string GetDisplayNames<T>(this IEnumerable<T> enumValues) where T : Enum =>
        string.Join(',', enumValues.Select(e => e.GetDisplayName()));
}