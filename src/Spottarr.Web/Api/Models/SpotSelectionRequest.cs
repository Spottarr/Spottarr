namespace Spottarr.Web.Api.Models;

/// <summary>
/// Selects the spots to mark. Exactly one of the selections must be provided.
/// </summary>
internal sealed record SpotSelectionRequest
{
    public IReadOnlyCollection<int>? SpotIds { get; init; }
    public DateTimeOffset? SpottedAfter { get; init; }
    public DateTimeOffset? SpottedBefore { get; init; }
    public bool All { get; init; }
}
