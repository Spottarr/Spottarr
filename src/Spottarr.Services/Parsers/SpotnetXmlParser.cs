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

    public static Task<ParserResult<SpotnetXml>> Parse(string xml) => Parse([xml]);

    public static async Task<ParserResult<SpotnetXml>> Parse(IList<string> xml)
    {
        xml = xml.Select(SpotnetXmlCleaner.Clean).ToList();

        try
        {
            using var reader = new StringEnumerableReader(xml);
            var result = await Parse(reader);
            return new ParserResult<SpotnetXml>(result);
        }
        catch (XmlException ex)
        {
            return new ParserResult<SpotnetXml>($"Invalid spot XML header: '{ex.Message}'");
        }
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