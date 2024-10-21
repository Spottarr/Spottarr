using System.Globalization;
using System.ServiceModel.Syndication;
using Spottarr.Data.Entities;
using Spottarr.Data.Entities.Enums;

namespace Spottarr.Web.Newznab;

internal static class NewznabMapper
{
    /// <summary>
    /// Creates a category specific RSS syndication item including any custom newznab attributes
    /// See: https://newznab.readthedocs.io/en/latest/misc/api.html#predefined-attributes
    /// </summary>
    public static SyndicationItem ToSyndicationItem(this Spot spot, Uri nzbUri)
    {
        var item = MapSpot(spot, nzbUri)
            .AddNewznabNzbUrl(nzbUri, spot.Bytes)
            .AddCategories(spot.NewznabCategories)
            .AddPublishDate(spot.SpottedAt);
        
        return spot.Type switch
        {
            SpotType.Image => MapImageSpot(spot, item),
            SpotType.Audio => MapAudioSpot(spot, item),
            SpotType.Game => MapGameSpot(spot, item),
            SpotType.Application => MapApplicationSpot(spot, item),
            _ => item
        };
    }
    
    /// <summary>
    /// Adds attributes valid for all categories
    /// </summary>
    private static SyndicationItem MapSpot(Spot spot, Uri spotUri) =>
        new SyndicationItem(spot.Title, spot.Description, spotUri, spot.Id.ToString(CultureInfo.InvariantCulture), spot.UpdatedAt)
            .AddNewznabAttribute("size", spot.Bytes.ToString(CultureInfo.InvariantCulture))
            .AddNewznabAttributes("category", spot.NewznabCategories.Select(c => ((int)c).ToString(CultureInfo.InvariantCulture)))
            .AddNewznabAttribute("guid", spot.Id.ToString(CultureInfo.InvariantCulture))
            .AddNewznabAttribute("files", null)
            .AddNewznabAttribute("poster", spot.Spotter)
            .AddNewznabAttribute("group", null)
            .AddNewznabAttribute("team", spot.Spotter)
            .AddNewznabAttribute("grabs", null)
            .AddNewznabAttribute("password", null)
            .AddNewznabAttribute("comments", null)
            .AddNewznabAttribute("usenetdate", spot.SpottedAt.ToString("r"))
            .AddNewznabAttribute("nfo", null)
            .AddNewznabAttribute("info", null)
            .AddNewznabAttribute("year", string.Join(',', spot.Years));

    /// <summary>
    /// Adds newznab attributes for tv, movies, books and erotic categories
    /// An image spot can have multiple types, e.g. Erotic AND Series
    /// </summary>
    private static SyndicationItem MapImageSpot(Spot spot, SyndicationItem item) =>
        spot.ImageTypes.Aggregate(item, (current, type) => MapImageSpotType(type, spot, current));

    private static SyndicationItem MapImageSpotType(ImageType type, Spot spot, SyndicationItem item) =>
        type switch
        {
            ImageType.Series => MapTvSpot(spot, item),
            ImageType.Movie => MapMovieSpot(spot, item),
            ImageType.Erotic => MapEroticSpot(spot, item),
            ImageType.Book => MapBookSpot(spot, item),
            _ => item,
        };

    /// <summary>
    /// Adds newznab attributes for tv category
    /// </summary>
    private static SyndicationItem MapTvSpot(Spot spot, SyndicationItem item) =>
        item.AddNewznabAttribute("season", string.Join(',', spot.Seasons))
            .AddNewznabAttribute("episode", string.Join(',', spot.Episodes))
            .AddNewznabAttribute("rageid", null)
            .AddNewznabAttribute("tvtitle", null)
            .AddNewznabAttribute("tvairdate", null)
            .AddNewznabAttribute("video", string.Join(',', spot.ImageFormats.Select(Enum.GetName)))
            .AddNewznabAttribute("audio", null)
            .AddNewznabAttribute("resolution", null)
            .AddNewznabAttribute("framerate", null)
            .AddNewznabAttribute("language", string.Join(',', spot.ImageLanguages.Select(Enum.GetName)))
            .AddNewznabAttribute("subs", string.Join(',', spot.ImageLanguages.Select(Enum.GetName)))
            .AddNewznabAttribute("genre", string.Join(',', spot.ImageGenres.Select(Enum.GetName)))
            .AddNewznabAttribute("coverurl", null)
            .AddNewznabAttribute("backdropcoverurl", null);

    /// <summary>
    /// Adds newznab attributes for movies category
    /// </summary>
    private static SyndicationItem MapMovieSpot(Spot spot, SyndicationItem item) =>
        item.AddNewznabAttribute("video", string.Join(',', spot.ImageFormats.Select(Enum.GetName)))
            .AddNewznabAttribute("audio", null)
            .AddNewznabAttribute("resolution", null)
            .AddNewznabAttribute("framerate", null)
            .AddNewznabAttribute("language", string.Join(',', spot.ImageLanguages.Select(Enum.GetName)))
            .AddNewznabAttribute("subs", string.Join(',', spot.ImageLanguages.Select(Enum.GetName)))
            .AddNewznabAttribute("imdb", null)
            .AddNewznabAttribute("imdbscore", null)
            .AddNewznabAttribute("imdbtitle", null)
            .AddNewznabAttribute("imdbtagline", null)
            .AddNewznabAttribute("imdbplot", null)
            .AddNewznabAttribute("imdbyear", null)
            .AddNewznabAttribute("imdbdirector", null)
            .AddNewznabAttribute("imdbactors", null)
            .AddNewznabAttribute("genre", string.Join(',', spot.ImageGenres.Select(Enum.GetName)))
            .AddNewznabAttribute("coverurl", null)
            .AddNewznabAttribute("backdropcoverurl", null)
            .AddNewznabAttribute("review", null);

    /// <summary>
    /// Adds newznab attributes for erotic (unofficial) category
    /// </summary>
    /// <returns></returns>
    private static SyndicationItem MapEroticSpot(Spot spot, SyndicationItem item) =>
        item.AddNewznabAttribute("video", string.Join(',', spot.ImageFormats.Select(Enum.GetName)))
            .AddNewznabAttribute("audio", null)
            .AddNewznabAttribute("resolution", null)
            .AddNewznabAttribute("framerate", null)
            .AddNewznabAttribute("language", string.Join(',', spot.ImageLanguages.Select(Enum.GetName)))
            .AddNewznabAttribute("subs", string.Join(',', spot.ImageLanguages.Select(Enum.GetName)))
            .AddNewznabAttribute("genre", string.Join(',', spot.ImageGenres.Select(Enum.GetName)))
            .AddNewznabAttribute("coverurl", null)
            .AddNewznabAttribute("backdropcoverurl", null);

    /// <summary>
    /// Adds newznab attributes for books category
    /// </summary>
    private static SyndicationItem MapBookSpot(Spot spot, SyndicationItem item) =>
        item.AddNewznabAttribute("publisher", null)
            .AddNewznabAttribute("coverurl", null)
            .AddNewznabAttribute("review", null)
            .AddNewznabAttribute("booktitle", null)
            .AddNewznabAttribute("publishdate", null)
            .AddNewznabAttribute("author", null)
            .AddNewznabAttribute("pages", null);

    /// <summary>
    /// Adds newznab attributes for audio category
    /// </summary>
    private static SyndicationItem MapAudioSpot(this Spot spot, SyndicationItem item)
    {
        return item.AddNewznabAttribute("audio", string.Join(',', spot.AudioFormats.Select(Enum.GetName)))
            .AddNewznabAttribute("language", null)

            .AddNewznabAttribute("artist", null)
            .AddNewznabAttribute("album", null)
            .AddNewznabAttribute("publisher", null)
            .AddNewznabAttribute("tracks", null)
            .AddNewznabAttribute("coverurl", null)
            .AddNewznabAttribute("backdropcoverurl", null)
            .AddNewznabAttribute("review", null);
    }
    
    /// <summary>
    /// Adds newznab attributes for game (unofficial) category
    /// </summary>
    private static SyndicationItem MapGameSpot(this Spot spot, SyndicationItem item) => item;

    /// <summary>
    /// Adds newznab attributes for pc (unofficial) category
    /// </summary>
    private static SyndicationItem MapApplicationSpot(this Spot spot, SyndicationItem item) => item;
}