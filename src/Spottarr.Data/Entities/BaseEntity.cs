namespace Spottarr.Data.Entities;

public abstract class BaseEntity
{
    public int Id { get; set; }

    public DateTime CreatedAt { get; init; }
    public DateTime UpdatedAt { get; set; }
}