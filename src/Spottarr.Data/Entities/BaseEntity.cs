using System.ComponentModel.DataAnnotations.Schema;

namespace Spottarr.Data.Entities;

public abstract class BaseEntity
{
    public int Id { get; set; }
    
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public DateTimeOffset CreatedAt { get; set; }
    [DatabaseGenerated(DatabaseGeneratedOption.Computed)]
    public DateTimeOffset UpdatedAt { get; set; }
}