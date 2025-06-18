using System.Xml.Serialization;

namespace Spottarr.Services.Spotnet;

public sealed class ImageSegment
{
    [XmlElement(ElementName = "Segment")] public string Segment { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "Height")]
    public int Height { get; set; }

    [XmlAttribute(AttributeName = "Width")]
    public int Width { get; set; }
}