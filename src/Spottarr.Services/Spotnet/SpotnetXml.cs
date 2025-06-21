using System.Xml;
using System.Xml.Serialization;
using Spottarr.Services.Helpers;

namespace Spottarr.Services.Spotnet;

[XmlRoot(ElementName = "Spotnet")]
public sealed class SpotnetXml : IXmlReadable<SpotnetXml>
{
    public const string HeaderName = "X-XML";

    [XmlElement(ElementName = "Posting")] public Posting Posting { get; set; } = null!;

    public static async Task<SpotnetXml> ReadXml(XmlReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);
        var result = new SpotnetXml();

        reader.ReadStartElement("Posting");
        result.Posting = await Posting.ReadXml(reader);
        reader.ReadEndElement();

        return result;
    }
}