namespace Spottarr.Services.Models;

/// <summary>
/// Selects the spots an operation applies to. Without any criteria it selects every spot.
/// </summary>
public sealed record SpotSelection
{
    public IReadOnlyCollection<int>? SpotIds { get; init; }
    public DateTimeOffset? SpottedAfter { get; init; }
    public DateTimeOffset? SpottedBefore { get; init; }
}
