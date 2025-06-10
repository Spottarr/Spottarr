using System.Xml;
using System.Xml.Serialization;
using Spottarr.Services.Helpers;
using Spottarr.Services.Spotnet;

namespace Spottarr.Services.Parsers;

internal class SpotnetXmlParser
{
    private static readonly XmlSerializer Serializer = new(typeof(SpotnetXml));

    private static readonly XmlReaderSettings XmlReaderSettings = new()
    {
        Async = true,
        // Spot XML headers often contain invalid characters
        CheckCharacters = false
    };

    public static SpotnetXml Parse(string xml)
    {
        using var reader = new StringReader(xml);
        return Parse(reader);
    }

    public static SpotnetXml Parse(IEnumerable<string> xml)
    {
        using var reader = new StringEnumerableReader(xml);
        return Parse(reader);
    }

    private static SpotnetXml Parse(TextReader reader)
    {
        SpotnetXml? result;

        try
        {
            using var xmlReader = XmlReader.Create(reader, XmlReaderSettings);
            result = Serializer.Deserialize(xmlReader) as SpotnetXml;
        }
        catch (InvalidOperationException ex)
        {
            throw new InvalidOperationException("Failed to deserialize Spot XML", ex);
        }

        return result ?? throw new InvalidOperationException("Failed to deserialize Spot XML");
    }
}