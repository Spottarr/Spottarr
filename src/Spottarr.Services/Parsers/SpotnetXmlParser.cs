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
        CheckCharacters = false,
    };

    public static Task<ParserResult<SpotnetXml>> Parse(
        string xml,
        CancellationToken cancellationToken
    ) => Parse([xml], cancellationToken);

    public static async Task<ParserResult<SpotnetXml>> Parse(
        IEnumerable<string> xml,
        CancellationToken cancellationToken
    )
    {
        try
        {
            // Clean and stream the segments one at a time so we never hold a concatenated copy of the
            // (possibly large, multi-valued) X-XML header in memory.
            using var reader = new SegmentTextReader(xml.Select(SpotnetXmlCleaner.Clean));
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
