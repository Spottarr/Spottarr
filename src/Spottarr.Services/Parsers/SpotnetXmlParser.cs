using System.Xml;
using Spottarr.Services.Helpers;
using Spottarr.Services.Spotnet;

namespace Spottarr.Services.Parsers;

internal class SpotnetXmlParser
{
    private static readonly XmlReaderSettings XmlReaderSettings = new()
    {
        Async = true,
        // Spot XML headers often contain invalid characters
        CheckCharacters = false
    };

    public static async Task<SpotnetXml> Parse(string xml)
    {
        using var reader = new StringReader(xml);
        return await Parse(reader);
    }

    public static async Task<SpotnetXml> Parse(IEnumerable<string> xml)
    {
        using var reader = new StringEnumerableReader(xml);
        return await Parse(reader);
    }

    private static async Task<SpotnetXml> Parse(TextReader textReader)
    {
        using var reader = XmlReader.Create(textReader, XmlReaderSettings);

        await reader.MoveToContentAsync();

        reader.ReadStartElement("Spotnet");
        var result = await SpotnetXml.ReadXml(reader);
        reader.ReadEndElement();

        return result;
    }
}