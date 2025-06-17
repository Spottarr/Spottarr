using System.Xml;
using System.Xml.Serialization;
using Spottarr.Services.Helpers;

namespace Spottarr.Web.Newznab.Models;

internal sealed class Searching : XmlWritable
{
    [XmlElement("search")] public required Search Search { get; init; }
    [XmlElement("tv-search")] public required Search TvSearch { get; init; }
    [XmlElement("movie-search")] public required Search MovieSearch { get; init; }
    [XmlElement("pc-search")] public required Search PcSearch { get; init; }
    [XmlElement("audio-search")] public required Search AudioSearch { get; init; }
    [XmlElement("book-search")] public required Search BookSearch { get; init; }

    public override void WriteXml(XmlWriter writer)
    {
        writer.WriteElement("search", Search);
        writer.WriteElement("tv-search", TvSearch);
        writer.WriteElement("movie-search", MovieSearch);
        writer.WriteElement("pc-search", PcSearch);
        writer.WriteElement("audio-search", AudioSearch);
        writer.WriteElement("book-search", BookSearch);
    }
}