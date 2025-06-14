using System.Xml.Serialization;

namespace Spottarr.Web.Newznab.Models;

public sealed class Searching
{
    [XmlElement("search")] public Search Search { get; set; } = null!;
    [XmlElement("tv-search")] public Search TvSearch { get; set; } = null!;
    [XmlElement("movie-search")] public Search MovieSearch { get; set; } = null!;
    [XmlElement("pc-search")] public Search PcSearch { get; set; } = null!;
    [XmlElement("audio-search")] public Search AudioSearch { get; set; } = null!;
    [XmlElement("book-search")] public Search BookSearch { get; set; } = null!;
}