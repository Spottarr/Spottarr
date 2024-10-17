using System.Xml.Serialization;

namespace Spottarr.Web.Newznab.Models;

public sealed class Searching
{
    [XmlElement("search")] public required Search Search { get; init; }
    [XmlElement("tv-search")] public required Search TvSearch { get; init; }
    [XmlElement("movie-search")] public required Search MovieSearch { get; init; }
    [XmlElement("pc-search")] public required Search PcSearch { get; init; }
    [XmlElement("audio-search")] public required Search AudioSearch { get; init; }
    [XmlElement("book-search")] public required Search BookSearch { get; init; }
}