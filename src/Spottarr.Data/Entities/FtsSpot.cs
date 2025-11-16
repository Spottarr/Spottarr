namespace Spottarr.Data.Entities;

public class FtsSpot : BaseFtsEntity
{
    public required string Title { get; init; }
    public required string Description { get; init; }
    public Spot? Spot { get; init; }
    public int SpotId { get; set; }
}