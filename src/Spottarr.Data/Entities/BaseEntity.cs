namespace Spottarr.Data.Entities;

public abstract class BaseEntity
{
    public int Id { get; set; }
    
    public required DateTimeOffset CreatedAt { get; init; }
    public required DateTimeOffset UpdatedAt { get; set; }
}