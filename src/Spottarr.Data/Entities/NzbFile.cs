namespace Spottarr.Data.Entities;

public class NzbFile : BaseEntity
{
    public Spot? Spot { get; init; }
    public int SpotId { get; set; }
    public required string MessageId { get; init; }
#pragma warning disable CA1819
    public required byte[] Data { get; init; }
#pragma warning restore CA1819
}