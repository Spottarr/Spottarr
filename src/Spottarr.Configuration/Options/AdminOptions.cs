using Spottarr.Configuration.Contracts;

namespace Spottarr.Configuration.Options;

public sealed class AdminOptions : IOptionsSection
{
    public static string Section => "Admin";

    /// <summary>
    /// The key required to call the administrative API. When unset the API is unavailable.
    /// </summary>
    public string ApiKey { get; init; } = string.Empty;
}
