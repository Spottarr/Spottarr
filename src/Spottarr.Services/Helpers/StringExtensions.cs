namespace Spottarr.Services.Helpers;

internal static class StringExtensions
{
    public static string Truncate(this string source, int maxLength) => source[..Math.Min(source.Length, maxLength)];
}