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
        if (reader.IsStartElement("Segment"))
            result.Segment = await reader.ReadElementContentAsStringAsync();

        return result;
    }
}