using System.Xml;

namespace Spottarr.Web.Helpers;

internal static class StringExtensions
{
    public static string SanitizeXmlString(this string source) =>
        string.Concat(source.Select(c => XmlConvert.IsXmlChar(c) ? c : '?'));
}