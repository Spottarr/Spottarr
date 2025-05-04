namespace Spottarr.Data.Configuration;

public class DatabaseOptions
{
    public const string Section = "Database";

    public required DatabaseProvider Provider { get; init; }

    public required string ConnectionString { get; init; }
}

public enum DatabaseProvider
{
    Sqlite,
    Postgres
}