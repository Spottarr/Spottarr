using System.Xml;
using System.Xml.Serialization;
using Spottarr.Services.Helpers;

namespace Spottarr.Services.Spotnet;

internal sealed class Image : IXmlReadable<Image>
{
    [XmlElement(ElementName = "Segment")]
    public List<string> Segments { get; } = [];

    [XmlAttribute(AttributeName = "Height")]
    public int Height { get; set; }

    [XmlAttribute(AttributeName = "Width")]
    public int Width { get; set; }

    public static async Task<Image> ReadXml(XmlReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);

        var result = new Image();

        if (int.TryParse(reader.GetAttribute("Width"), out var width))
            result.Width = width;
        if (int.TryParse(reader.GetAttribute("Height"), out var height))
            result.Height = height;

        var depth = reader.Depth;

        while (reader.Depth >= depth)
        {
            if (reader.NodeType != XmlNodeType.Element)
            {
                await reader.ReadAsync();
                continue;
            }

            switch (reader.Name)
            {
                case "Segment":
                    result.Segments.Add(await reader.ReadElementContentAsStringAsync());
                    break;
                default:
                    await reader.SkipAsync(); // Skip unknown elements
                    break;
            }
        }

        return result;
    }
}
