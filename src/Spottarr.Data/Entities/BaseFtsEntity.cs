namespace Spottarr.Data.Entities;

/// <summary>
/// Base entity for full text search (FTS5)
/// See: https://www.bricelam.net/2020/08/08/sqlite-fts-and-efcore.html
/// </summary>
public abstract class BaseFtsEntity
{
    public int RowId { get; set; }
    public string? Match { get; set; }
    public double? Rank { get; set; }
}