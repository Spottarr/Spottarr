using Spottarr.Data.Entities.Fts;

namespace Spottarr.Data.Entities;

public class FtsSpot : ISqliteFtsEntity
{
    public required string Title { get; init; }
    public required string Description { get; init; }
    public Spot? Spot { get; init; }
    public int SpotId { get; set; }
    public string? Match { get; set; }
    public double? Rank { get; set; }
}