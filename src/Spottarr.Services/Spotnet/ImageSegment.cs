using System.Xml.Serialization;

namespace Spottarr.Services.Spotnet;

public sealed class ImageSegment
{
    [XmlElement(ElementName = "Segment")] public required string Segment { get; init; }

    [XmlAttribute(AttributeName = "Height")] public int Height { get; init; }

    [XmlAttribute(AttributeName = "Width")] public int Width { get; init; }
}