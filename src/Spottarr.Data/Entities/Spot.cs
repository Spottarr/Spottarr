using Spottarr.Data.Entities.Enums;

namespace Spottarr.Data.Entities;

public abstract class Spot : BaseEntity
{
    public required string Title { get; set; }
    public string? Description { get; set; }
    public required string Spotter { get; set; }
    public required long Bytes { get; set; }
    public required string MessageId { get; set; }
    public required long MessageNumber { get; set; }
    public SpotType Type { get; set; }
    public FtsSpot? FtsSpot { get; set; }
    public ICollection<int> Years { get; init; } = [];
    public ICollection<int> Seasons { get; init; } = [];
    public ICollection<int> Episodes { get; init; } = [];
    public required DateTime SpottedAt { get; set; }
    public DateTime? IndexedAt { get; set; }
}