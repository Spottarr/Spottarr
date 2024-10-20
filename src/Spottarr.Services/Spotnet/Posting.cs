using System.Xml.Serialization;

namespace Spottarr.Services.Spotnet;

public sealed class Posting
{
    [XmlElement(ElementName = "Key")] public int Key { get; init; }

    [XmlElement(ElementName = "Created")] public int Created { get; init; }

    [XmlElement(ElementName = "Poster")] public required string Poster { get; init; }

    [XmlElement(ElementName = "Title")] public required string Title { get; init; }

    [XmlElement(ElementName = "Description")] public required string Description { get; init; }

    [XmlElement(ElementName = "Image")] public required ImageSegment Image { get; init; }

    [XmlElement(ElementName = "Size")] public long Size { get; init; }

    [XmlElement(ElementName = "Category")] public required Category Category { get; init; }

    [XmlElement(ElementName = "NZB")] public required NzbSegment Nzb { get; init; }
}