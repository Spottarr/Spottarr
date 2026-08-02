namespace Spottarr.Web.Api.Models;

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
    /// Unset while the spot is marked for a reindex.
    /// </summary>
    public DateTimeOffset? IndexedAt { get; init; }

    /// <summary>
    /// Unset while the spot is marked for a reimport.
    /// </summary>
    public DateTimeOffset? ImportedAt { get; init; }
}
