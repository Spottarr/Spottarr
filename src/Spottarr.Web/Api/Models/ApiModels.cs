namespace Spottarr.Web.Api.Models;

/// <summary>
/// Selects the spots to flag. Exactly one of the selections must be provided.
/// </summary>
internal sealed record SpotSelectionRequest
{
    /// <summary>
    /// The ids of the spots to flag.
    /// </summary>
    public IReadOnlyCollection<int>? SpotIds { get; init; }

    /// <summary>
    /// Flags spots that were spotted on or after this date.
    /// </summary>
    public DateTimeOffset? SpottedAfter { get; init; }

    /// <summary>
    /// Flags spots that were spotted on or before this date.
    /// </summary>
    public DateTimeOffset? SpottedBefore { get; init; }

    /// <summary>
    /// Flags every spot.
    /// </summary>
    public bool All { get; init; }
}

internal sealed record SpotFlagStatusResponse
{
    /// <summary>
    /// The number of spots that are waiting to be processed.
    /// </summary>
    public required int Flagged { get; init; }

    /// <summary>
    /// Whether the import job that processes flagged spots is running.
    /// </summary>
    public required bool Running { get; init; }
}

internal sealed record SpotFlaggedResponse
{
    public required int Flagged { get; init; }
}

internal sealed record SpotFlagsClearedResponse
{
    public required int Cleared { get; init; }
}

internal sealed record SpotResponse
{
    public required int Id { get; init; }
    public required string Title { get; init; }
    public string? ReleaseTitle { get; init; }
    public string? Description { get; init; }
    public string? Tag { get; init; }
    public Uri? Url { get; init; }
    public string? Filename { get; init; }
    public string? Newsgroup { get; init; }
    public required string Spotter { get; init; }
    public required long Bytes { get; init; }
    public required string MessageId { get; init; }
    public required long MessageNumber { get; init; }
    public required IReadOnlyCollection<string> NzbMessageIds { get; init; }
    public required IReadOnlyCollection<string> ImageMessageIds { get; init; }
    public required string Type { get; init; }
    public required IReadOnlyCollection<int> NewznabCategories { get; init; }
    public required IReadOnlyCollection<int> Years { get; init; }
    public required IReadOnlyCollection<int> Seasons { get; init; }
    public required IReadOnlyCollection<int> Episodes { get; init; }
    public string? ImdbId { get; init; }
    public string? TvdbId { get; init; }
    public required DateTimeOffset SpottedAt { get; init; }
    public required DateTimeOffset CreatedAt { get; init; }
    public required DateTimeOffset UpdatedAt { get; init; }

    /// <summary>
    /// Unset while the spot is flagged for a reindex.
    /// </summary>
    public DateTimeOffset? IndexedAt { get; init; }

    /// <summary>
    /// Unset while the spot is flagged for a reimport.
    /// </summary>
    public DateTimeOffset? ImportedAt { get; init; }
}
