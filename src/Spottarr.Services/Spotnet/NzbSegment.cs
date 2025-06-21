using System.Xml;
using System.Xml.Serialization;
using Spottarr.Services.Helpers;

namespace Spottarr.Services.Spotnet;

public class NzbSegment : IXmlReadable<NzbSegment>
{
    [XmlElement(ElementName = "Segment")] public string Segment { get; set; } = string.Empty;

    public static async Task<NzbSegment> ReadXml(XmlReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);

        var result = new NzbSegment();
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
                    result.Segment = await reader.ReadElementContentAsStringAsync();
                    break;
                default:
                    await reader.SkipAsync(); // Skip unknown elements
                    break;
            }
        }

        return result;
    }
}