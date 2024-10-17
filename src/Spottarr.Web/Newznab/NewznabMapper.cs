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
    public static SyndicationItem ToSyndicationItem(this Spot spot, Uri baseUri)
    {
        var item = MapSpot(spot, baseUri)
            .AddCategory("test123")
            .AddNewznabNzbUrl(new Uri("https://www.test.com/test.nzb"), spot.Bytes);

        item.PublishDate = spot.SpottedAt;
        
        return spot switch
        {
            ImageSpot imageSpot => MapImageSpot(imageSpot, item),
            AudioSpot audioSpot => MapAudioSpot(audioSpot, item),
            GameSpot gameSpot => MapGameSpot(gameSpot, item),
            ApplicationSpot applicationSpot => MapApplicationSpot(applicationSpot, item),
            _ => item
        };
    }
    
    /// <summary>
    /// Adds attributes valid for all categories
    /// </summary>
    private static SyndicationItem MapSpot(Spot spot, Uri baseUri) =>
        new SyndicationItem(spot.Subject, spot.Description, baseUri, spot.MessageId, spot.UpdatedAt)
            .AddNewznabAttribute("size", spot.Bytes.ToString(CultureInfo.InvariantCulture))
            .AddNewznabAttribute("category", "x")
            .AddNewznabAttribute("guid", spot.MessageId)
            .AddNewznabAttribute("files", "TBD")
            .AddNewznabAttribute("poster", spot.Spotter)
            .AddNewznabAttribute("group", "TBD")
            .AddNewznabAttribute("team", spot.Spotter)
            .AddNewznabAttribute("grabs", "TBD")
            .AddNewznabAttribute("password", "TBD")
            .AddNewznabAttribute("comments", "TBD")
            .AddNewznabAttribute("usenetdate", spot.SpottedAt.ToString("r"))
            .AddNewznabAttribute("nfo", "TBD")
            .AddNewznabAttribute("info", "TBD")
            .AddNewznabAttribute("year", "TBD");

    /// <summary>
    /// Adds newznab attributes for tv, movies, books and erotic categories
    /// An image spot can have multiple types, e.g. Erotic AND Series
    /// </summary>
    private static SyndicationItem MapImageSpot(ImageSpot spot, SyndicationItem item) =>
        spot.ImageTypes.Aggregate(item, (current, type) => MapImageSpotType(type, spot, current));

    private static SyndicationItem MapImageSpotType(ImageType type, ImageSpot spot, SyndicationItem item) =>
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
    private static SyndicationItem MapTvSpot(ImageSpot spot, SyndicationItem item) =>
        item.AddNewznabAttribute("season", "TBD")
            .AddNewznabAttribute("episode", "TBD")
            .AddNewznabAttribute("rageid", "TBD")
            .AddNewznabAttribute("tvtitle", "TBD")
            .AddNewznabAttribute("tvairdate", "TBD")
            .AddNewznabAttribute("video", string.Join(',', spot.ImageFormats.Select(Enum.GetName)))
            .AddNewznabAttribute("audio", "TBD")
            .AddNewznabAttribute("resolution", "TBD")
            .AddNewznabAttribute("framerate", "TBD")
            .AddNewznabAttribute("language", string.Join(',', spot.ImageLanguages.Select(Enum.GetName)))
            .AddNewznabAttribute("subs", string.Join(',', spot.ImageLanguages.Select(Enum.GetName)))
            .AddNewznabAttribute("genre", string.Join(',', spot.ImageGenres.Select(Enum.GetName)))
            .AddNewznabAttribute("coverurl", "TBD")
            .AddNewznabAttribute("backdropcoverurl", "TBD");

    /// <summary>
    /// Adds newznab attributes for movies category
    /// </summary>
    private static SyndicationItem MapMovieSpot(ImageSpot spot, SyndicationItem item) =>
        item.AddNewznabAttribute("video", string.Join(',', spot.ImageFormats.Select(Enum.GetName)))
            .AddNewznabAttribute("audio", "TBD")
            .AddNewznabAttribute("resolution", "TBD")
            .AddNewznabAttribute("framerate", "TBD")
            .AddNewznabAttribute("language", string.Join(',', spot.ImageLanguages.Select(Enum.GetName)))
            .AddNewznabAttribute("subs", string.Join(',', spot.ImageLanguages.Select(Enum.GetName)))
            .AddNewznabAttribute("imdb", "TBD")
            .AddNewznabAttribute("imdbscore", "TBD")
            .AddNewznabAttribute("imdbtitle", "TBD")
            .AddNewznabAttribute("imdbtagline", "TBD")
            .AddNewznabAttribute("imdbplot", "TBD")
            .AddNewznabAttribute("imdbyear", "TBD")
            .AddNewznabAttribute("imdbdirector", "TBD")
            .AddNewznabAttribute("imdbactors", "TBD")
            .AddNewznabAttribute("genre", string.Join(',', spot.ImageGenres.Select(Enum.GetName)))
            .AddNewznabAttribute("coverurl", "TBD")
            .AddNewznabAttribute("backdropcoverurl", "TBD")
            .AddNewznabAttribute("review", "TBD");

    /// <summary>
    /// Adds newznab attributes for erotic (unofficial) category
    /// </summary>
    /// <returns></returns>
    private static SyndicationItem MapEroticSpot(ImageSpot spot, SyndicationItem item) =>
        item.AddNewznabAttribute("video", string.Join(',', spot.ImageFormats.Select(Enum.GetName)))
            .AddNewznabAttribute("audio", "TBD")
            .AddNewznabAttribute("resolution", "TBD")
            .AddNewznabAttribute("framerate", "TBD")
            .AddNewznabAttribute("language", string.Join(',', spot.ImageLanguages.Select(Enum.GetName)))
            .AddNewznabAttribute("subs", string.Join(',', spot.ImageLanguages.Select(Enum.GetName)))
            .AddNewznabAttribute("genre", string.Join(',', spot.ImageGenres.Select(Enum.GetName)))
            .AddNewznabAttribute("coverurl", "TBD")
            .AddNewznabAttribute("backdropcoverurl", "TBD");

    /// <summary>
    /// Adds newznab attributes for books category
    /// </summary>
    private static SyndicationItem MapBookSpot(ImageSpot spot, SyndicationItem item) =>
        item.AddNewznabAttribute("publisher", "TBD")
            .AddNewznabAttribute("coverurl", "TBD")
            .AddNewznabAttribute("review", "TBD")
            .AddNewznabAttribute("booktitle", "TBD")
            .AddNewznabAttribute("publishdate", "TBD")
            .AddNewznabAttribute("author", "TBD")
            .AddNewznabAttribute("pages", "TBD");

    /// <summary>
    /// Adds newznab attributes for audio category
    /// </summary>
    private static SyndicationItem MapAudioSpot(this AudioSpot spot, SyndicationItem item)
    {
        return item.AddNewznabAttribute("audio", string.Join(',', spot.AudioFormats.Select(Enum.GetName)))
            .AddNewznabAttribute("language", "TBD")

            .AddNewznabAttribute("artist", "TBD")
            .AddNewznabAttribute("album", "TBD")
            .AddNewznabAttribute("publisher", "TBD")
            .AddNewznabAttribute("tracks", "TBD")
            .AddNewznabAttribute("coverurl", "TBD")
            .AddNewznabAttribute("backdropcoverurl", "TBD")
            .AddNewznabAttribute("review", "TBD");
    }
    
    /// <summary>
    /// Adds newznab attributes for game (unofficial) category
    /// </summary>
    private static SyndicationItem MapGameSpot(this GameSpot spot, SyndicationItem item) => item;

    /// <summary>
    /// Adds newznab attributes for pc (unofficial) category
    /// </summary>
    private static SyndicationItem MapApplicationSpot(this ApplicationSpot spot, SyndicationItem item) => item;
}