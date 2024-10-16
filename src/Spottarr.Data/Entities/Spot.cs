using Spottarr.Data.Entities.Enums;

namespace Spottarr.Data.Entities;

public abstract class Spot : BaseEntity
{
    public required string Subject { get; set; }
    public required string Spotter { get; set; }
    
    public required long Bytes { get; set; }
    public required string MessageId { get; set; }
    public required long MessageNumber { get; set; }
    
    public SpotType Type { get; set; }
    
    public required DateTime SpottedAt { get; set; }
}