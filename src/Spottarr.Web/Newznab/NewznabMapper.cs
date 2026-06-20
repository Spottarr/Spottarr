using System.Globalization;
using System.ServiceModel.Syndication;
using Spottarr.Data.Entities;
using Spottarr.Data.Entities.Enums;
using Spottarr.Web.Helpers;

namespace Spottarr.Web.Newznab;

internal static class NewznabMapper
{
    /// <summary>
    /// Creates a category specific RSS syndication item including any custom newznab attributes
    /// See: https://newznab.readthedocs.io/en/latest/misc/api.html#predefined-attributes
    /// </summary>
    public static SyndicationItem ToSyndicationItem(this Spot spot, Uri detailsUri, Uri nzbUri)
    {
        var item = MapSpot(spot, detailsUri, nzbUri)
            .AddNewznabNzbUrl(nzbUri, spot.Bytes)
            .AddCategories(spot.NewznabCategories)
            .AddPublishDate(spot.SpottedAt);

        if (!string.IsNullOrEmpty(spot.ReleaseTitle))
        {
            item.Title =
                spot.Title == spot.ReleaseTitle
                    ? new TextSyndicationContent(spot.ReleaseTitle)
                    : new TextSyndicationContent(
                        $"{spot.ReleaseTitle} | {spot.Title.SanitizeXmlString()}"
                    );

            item.AddNewznabAttribute("title", spot.Title);
        }

        return spot.Type switch
        {
            SpotType.Image => spot.MapImageSpot(item),
            SpotType.Audio => spot.MapAudioSpot(item),
            SpotType.Game => spot.MapGameSpot(item),
            SpotType.Application => spot.MapApplicationSpot(item),
            _ => item,
        };
    }

    /// <summary>
    /// Adds attributes valid for all categories.
    /// Newznab also defines these optional attributes which we do not currently populate:
    /// files, group, grabs, password, comments, nfo, info.
    /// </summary>
    private static SyndicationItem MapSpot(Spot spot, Uri detailsUri, Uri nzbUri) =>
        new SyndicationItem(
            spot.Title.SanitizeXmlString(),
            spot.Description?.SanitizeXmlString(),
            nzbUri,
            spot.Id.ToString(CultureInfo.InvariantCulture),
            spot.UpdatedAt
        )
            .AddNewznabDetailsUrl(detailsUri)
            .AddNewznabAttribute("size", spot.Bytes.ToString(CultureInfo.InvariantCulture))
            .AddNewznabAttributes(
                "category",
                spot.NewznabCategories.Select(c => ((int)c).ToString(CultureInfo.InvariantCulture))
            )
            .AddNewznabAttribute("guid", spot.Id.ToString(CultureInfo.InvariantCulture))
            .AddNewznabAttribute("poster", spot.Spotter)
            .AddNewznabAttribute("team", spot.Spotter)
            .AddNewznabAttribute("usenetdate", spot.SpottedAt.ToString("r"))
            .AddNewznabAttribute("year", string.Join(',', spot.Years));

    /// <summary>
    /// Adds newznab attributes for tv, movies, books and erotic categories
    /// An image spot can have multiple types, e.g. Erotic AND Series
    /// </summary>
    private static SyndicationItem MapImageSpot(this Spot spot, SyndicationItem item) =>
        spot.ImageTypes.Aggregate(item, (current, type) => spot.MapImageSpotType(type, current));

    private static SyndicationItem MapImageSpotType(
        this Spot spot,
        ImageType type,
        SyndicationItem item
    ) =>
        type switch
        {
            ImageType.Series => spot.MapTvSpot(spot.MapVideoSpot(item)),
            ImageType.Movie => spot.MapMovieSpot(spot.MapVideoSpot(item)),
            ImageType.Erotic => spot.MapVideoSpot(item),
            ImageType.Book => spot.MapBookSpot(item),
            _ => item,
        };

    /// <summary>
    /// Adds common newznab attributes for video formats.
    /// Newznab also defines audio, resolution, framerate, coverurl and backdropcoverurl which we do not populate.
    /// </summary>
    private static SyndicationItem MapVideoSpot(this Spot spot, SyndicationItem item) =>
        item.AddNewznabAttribute("video", string.Join(',', spot.ImageFormats.Select(Enum.GetName)))
            .AddNewznabAttribute("language", MapImageAudioLanguage(spot.ImageLanguages))
            .AddNewznabAttribute("subs", MapImageSubtitleLanguage(spot.ImageLanguages))
            .AddNewznabAttribute("genre", string.Join(',', spot.ImageGenres.Select(Enum.GetName)));

    /// <summary>
    /// Adds newznab attributes for tv category.
    /// Newznab also defines rageid, tvtitle and tvairdate which we do not populate.
    /// </summary>
    private static SyndicationItem MapTvSpot(this Spot spot, SyndicationItem item) =>
        item.AddNewznabAttribute("season", string.Join(',', spot.Seasons))
            .AddNewznabAttribute("episode", string.Join(',', spot.Episodes))
            .AddNewznabAttribute(
                "imdb",
                spot.ImdbId?.Replace("tt", string.Empty, StringComparison.OrdinalIgnoreCase)
            );

    /// <summary>
    /// Adds newznab attributes for movies category.
    /// Newznab also defines imdbscore, imdbtitle, imdbtagline, imdbplot, imdbyear, imdbdirector,
    /// imdbactors and review which we do not populate.
    /// </summary>
    private static SyndicationItem MapMovieSpot(this Spot spot, SyndicationItem item) =>
        item.AddNewznabAttribute(
            "imdb",
            spot.ImdbId?.Replace("tt", string.Empty, StringComparison.OrdinalIgnoreCase)
        );

    /// <summary>
    /// Adds newznab attributes for books category.
    /// Newznab defines publisher, coverurl, review, booktitle, publishdate, author and pages here,
    /// none of which we currently populate.
    /// </summary>
    private static SyndicationItem MapBookSpot(this Spot _, SyndicationItem item) => item;

    /// <summary>
    /// Adds newznab attributes for audio category.
    /// Newznab also defines language, artist, album, publisher, tracks, coverurl, backdropcoverurl
    /// and review which we do not populate.
    /// </summary>
    private static SyndicationItem MapAudioSpot(this Spot spot, SyndicationItem item) =>
        item.AddNewznabAttribute("audio", string.Join(',', spot.AudioFormats.Select(Enum.GetName)));

    /// <summary>
    /// Adds newznab attributes for game (unofficial) category
    /// </summary>
    private static SyndicationItem MapGameSpot(this Spot _, SyndicationItem item) => item;

    /// <summary>
    /// Adds newznab attributes for pc (unofficial) category
    /// </summary>
    private static SyndicationItem MapApplicationSpot(this Spot _, SyndicationItem item) => item;

    private static string MapImageAudioLanguage(ICollection<ImageLanguage> languages) =>
        string.Join(
            ',',
            languages
                .Select(l =>
                    l switch
                    {
                        ImageLanguage.EnglishAudio => "English",
                        ImageLanguage.DutchAudio => "Dutch",
                        ImageLanguage.GermanAudioWritten => "German",
                        ImageLanguage.FrenchAudioWritten => "French",
                        ImageLanguage.SpanishAudioWritten => "Spanish",
                        ImageLanguage.AsianAudioWritten => "Japanese", // Asian, lol
                        _ => null,
                    }
                )
                .Where(s => s != null)
        );

    private static string MapImageSubtitleLanguage(ICollection<ImageLanguage> languages) =>
        string.Join(
            ',',
            languages
                .Select(l =>
                    l switch
                    {
                        ImageLanguage.DutchSubtitlesExternal => "Dutch",
                        ImageLanguage.DutchSubtitlesBakedIn => "Dutch",
                        ImageLanguage.EnglishSubtitlesExternal => "English",
                        ImageLanguage.EnglishSubtitlesBakedIn => "English",
                        ImageLanguage.DutchSubtitlesConfigurable => "Dutch",
                        ImageLanguage.EnglishSubtitlesConfigurable => "English",
                        ImageLanguage.GermanAudioWritten => "German",
                        ImageLanguage.FrenchAudioWritten => "French",
                        ImageLanguage.SpanishAudioWritten => "Spanish",
                        ImageLanguage.AsianAudioWritten => "Japanese", // Asian, lol
                        _ => null,
                    }
                )
                .Where(s => s != null)
        );
}
