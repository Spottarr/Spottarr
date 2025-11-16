namespace Spottarr.Data.Entities.Fts;

/// <summary>
/// Fields required for full text search (FTS5) in SQLite
/// See: https://www.bricelam.net/2020/08/08/sqlite-fts-and-efcore.html
/// </summary>
public interface ISqliteFtsEntity
{
    string? Match { get; set; }
    double? Rank { get; set; }
}