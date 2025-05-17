using System.Diagnostics.CodeAnalysis;
using Spottarr.Data.Entities;
using Spottarr.Web.Helpers;

namespace Spottarr.Web.Models;

public class SpotTableRowResponseDto
{
    [SetsRequiredMembers]
    public SpotTableRowResponseDto(Spot spot)
    {
        ArgumentNullException.ThrowIfNull(spot);

        Id = spot.Id;
        Category = spot.ImageFormats.GetDisplayNames();
        Title = spot.Title;
        Genre = spot.ImageGenres.GetDisplayNames();
        Spotter = spot.Spotter;
        SpottedAt = spot.SpottedAt;
        Bytes = spot.Bytes;
    }

    public required int Id { get; init; }
    public required string Category { get; init; }
    public required string Title { get; init; }
    public required string Genre { get; init; }
    public required string Spotter { get; init; }
    public required DateTimeOffset SpottedAt { get; init; }
    public required long Bytes { get; init; }
}