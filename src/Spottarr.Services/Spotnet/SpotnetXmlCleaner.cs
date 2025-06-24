using System.Text.RegularExpressions;

namespace Spottarr.Services.Spotnet;

internal static partial class SpotnetXmlCleaner
{
    [GeneratedRegex("(ImageWidth)|('Height=)|(\"Height=)|(Image$)|('$)|(\"$)", RegexOptions.IgnoreCase)]
    private static partial Regex SpotnetInvalidXmlRegex();

    /// <summary>
    /// Fixes common issues with the spot XML image element.
    /// </summary>
    /// <param name="xml"></param>
    /// <returns></returns>
    internal static string Clean(string xml) => SpotnetInvalidXmlRegex().Replace(xml, match =>
    {
        return match.Value switch
        {
            "ImageWidth" => "Image Width",
            "'Height=" => "' Height=",
            "\"Height=" => "\" Height=",
            "Image" => "Image ",
            "'" => "' ",
            "\"" => "\" ",
            _ => match.Value
        };
    });
}