using System.Collections.Frozen;
using Spottarr.Data.Entities;
using Spottarr.Data.Entities.Enums;

namespace Spottarr.Services.Helpers;

internal static class SpotExtensions
{
    private static readonly FrozenSet<ImageGenre> AdultImageGenres =
    [
        ImageGenre.BiLegacy,
        ImageGenre.LesbianLegacy,
        ImageGenre.GayLegacy,
        ImageGenre.StraightLegacy,
        ImageGenre.Bi,
        ImageGenre.Lesbian,
        ImageGenre.Gay,
        ImageGenre.Straight,
        ImageGenre.Amateur,
        ImageGenre.Group,
        ImageGenre.Pov,
        ImageGenre.Solo,
        ImageGenre.Young,
        ImageGenre.Soft,
        ImageGenre.Fetish,
        ImageGenre.Old,
        ImageGenre.Bbw,
        ImageGenre.Sm,
        ImageGenre.Rough,
        ImageGenre.Ebony,
        ImageGenre.Hentai,
        ImageGenre.Outdoors
    ];

    public static bool IsAdultContent(this Spot spot) => spot.Type switch
    {
        SpotType.Image => AdultImageGenres.Intersect(spot.ImageGenres).Any(),
        _ => false
    };
}