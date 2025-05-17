using System.Xml.Serialization;

namespace Spottarr.Services.Spotnet;

public sealed class Posting
{
    [XmlElement(ElementName = "Key")] public int Key { get; init; }
    [XmlElement(ElementName = "Created")] public int Created { get; init; }
    [XmlElement(ElementName = "Poster")] public required string Poster { get; init; }
    [XmlElement(ElementName = "Title")] public required string Title { get; init; }

    [XmlElement(ElementName = "Description")]
    public required string Description { get; init; }

    [XmlElement(ElementName = "Tag")] public required string Tag { get; init; }
    [XmlElement(ElementName = "Website")] public required string Website { get; init; }
    [XmlElement(ElementName = "Image")] public required ImageSegment Image { get; init; }
    [XmlElement(ElementName = "Size")] public long Size { get; init; }
    [XmlElement(ElementName = "Category")] public required Category Category { get; init; }
    [XmlElement(ElementName = "NZB")] public required NzbSegment Nzb { get; init; }

    /* Optional fields
     not listed on https://github.com/spotnet/spotnet/wiki/Spot-Xml-format
     but present in https://github.com/spotweb/spotweb/blob/develop/lib/services/Format/Services_Format_Parsing.php */
    [XmlElement(ElementName = "Filename")] public required string Filename { get; init; }

    [XmlElement(ElementName = "Newsgroup")]
    public required string Newsgroup { get; init; }
}