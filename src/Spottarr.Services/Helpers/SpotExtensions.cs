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

    /// <summary>
    /// There are a large number of spots with a very large dummy description.
    /// These were posted under the tags listed below and can safely be ignored.
    /// </summary>
    private static readonly FrozenSet<string> TestTags =
    [
        "test",
        "tester",
        "timskuik"
    ];

    public static bool IsAdultContent(this Spot spot) => spot.Type switch
    {
        SpotType.Image => AdultImageGenres.Intersect(spot.ImageGenres).Any(),
        _ => false
    };

    public static bool IsTest(this Spot spot) => spot.Tag != null && TestTags.Contains(spot.Tag);
}