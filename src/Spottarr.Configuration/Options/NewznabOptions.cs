using Spottarr.Configuration.Contracts;

namespace Spottarr.Configuration.Options;

public sealed class NewznabOptions : IOptionsSection
{
    public static string Section => "Newznab";

    public string ApiKey { get; init; } = string.Empty;
}