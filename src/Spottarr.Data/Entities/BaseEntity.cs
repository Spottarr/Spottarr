namespace Spottarr.Data.Entities;

public abstract class BaseEntity
{
    public int Id { get; set; }
    
    public required DateTime CreatedAt { get; init; }
    public required DateTime UpdatedAt { get; set; }
}