using System.Xml;
using System.Xml.Serialization;
using Spottarr.Services.Helpers;

namespace Spottarr.Services.Spotnet;

internal sealed class Nzb : IXmlReadable<Nzb>
{
    [XmlElement(ElementName = "Segment")]
    public List<string> Segments { get; } = [];

    public static async Task<Nzb> ReadXml(XmlReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);

        var result = new Nzb();
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
