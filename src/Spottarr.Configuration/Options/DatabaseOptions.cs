using Spottarr.Configuration.Contracts;

namespace Spottarr.Configuration.Options;

public sealed class DatabaseOptions : IOptionsSection
{
    public static string Section => "Database";

    public required DatabaseProvider Provider { get; init; }

    public required string ConnectionString { get; init; }
}

public enum DatabaseProvider
{
    Sqlite,
    Postgres
}