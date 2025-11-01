namespace Spottarr.Services.Parsers;

/// <summary>
/// Cleans up a spot description for better indexing in full text search by removing BB tags
/// </summary>
internal static class BbCodeParser
{
    public static string Parse(string? description) =>
        description?.Replace("[br]", "\n", StringComparison.OrdinalIgnoreCase) ?? string.Empty;
}