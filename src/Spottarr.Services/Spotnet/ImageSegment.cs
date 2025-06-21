using System.Xml;
using System.Xml.Serialization;
using Spottarr.Services.Helpers;

namespace Spottarr.Services.Spotnet;

public sealed class ImageSegment : IXmlReadable<ImageSegment>
{
    [XmlElement(ElementName = "Segment")] public string Segment { get; set; } = string.Empty;

    [XmlAttribute(AttributeName = "Height")]
    public int Height { get; set; }

    [XmlAttribute(AttributeName = "Width")]
    public int Width { get; set; }

    public static async Task<ImageSegment> ReadXml(XmlReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);

        var result = new ImageSegment();

        if (int.TryParse(reader.GetAttribute("Width"), out var width))
            result.Width = width;
        if (int.TryParse(reader.GetAttribute("Height"), out var height))
            result.Height = height;

        if (reader.IsStartElement("Segment"))
            result.Segment = await reader.ReadElementContentAsStringAsync();

        return result;
    }
}