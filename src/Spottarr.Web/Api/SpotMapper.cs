using Spottarr.Data.Entities;
using Spottarr.Web.Api.Models;

namespace Spottarr.Web.Api;

internal static class SpotMapper
{
    public static SpotResponse ToResponse(this Spot spot) =>
        new()
        {
            Id = spot.Id,
            Title = spot.Title,
            ReleaseTitle = spot.ReleaseTitle,
            Description = spot.Description,
            Tag = spot.Tag,
            Url = spot.Url,
            Filename = spot.Filename,
            Newsgroup = spot.Newsgroup,
            Spotter = spot.Spotter,
            Bytes = spot.Bytes,
            MessageId = spot.MessageId,
            MessageNumber = spot.MessageNumber,
            NzbMessageIds = [.. spot.NzbMessageIds],
            ImageMessageIds = [.. spot.ImageMessageIds],
            Type = spot.Type.ToString(),
            NewznabCategories = [.. spot.NewznabCategories.Select(c => (int)c)],
            Years = [.. spot.Years],
            Seasons = [.. spot.Seasons],
            Episodes = [.. spot.Episodes],
            ImdbId = spot.ImdbId,
            TvdbId = spot.TvdbId,
            SpottedAt = spot.SpottedAt,
            CreatedAt = spot.CreatedAt,
            UpdatedAt = spot.UpdatedAt,
            IndexedAt = spot.IndexedAt,
            ImportedAt = spot.ImportedAt,
        };
}
