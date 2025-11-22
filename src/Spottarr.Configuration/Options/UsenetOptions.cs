using Spottarr.Configuration.Contracts;

namespace Spottarr.Configuration.Options;

public sealed class UsenetOptions : IOptionsSection
{
    public static string Section => "Usenet";

    public required string Hostname { get; init; }
    public required string Username { get; init; }
    public required string Password { get; init; }
    public required int Port { get; init; }
    public required bool UseTls { get; init; }
    public required int MaxConnections { get; init; }
}