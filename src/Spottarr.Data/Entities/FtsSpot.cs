namespace Spottarr.Data.Entities;

public class FtsSpot : BaseFtsEntity
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public Spot? Spot { get; set; }
}