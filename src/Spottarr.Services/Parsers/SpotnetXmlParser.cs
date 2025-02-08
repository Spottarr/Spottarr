using System.Xml;
using System.Xml.Serialization;
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
        SpotnetXml? result;
        
        try
        {
            using var stringReader = new StringReader(xml);
            using var xmlReader = XmlReader.Create(stringReader, XmlReaderSettings);
            result = Serializer.Deserialize(xmlReader) as SpotnetXml;
        }
        catch (InvalidOperationException ex)
        {
            throw new InvalidOperationException(xml, ex);
        }

        if (result == null) throw new InvalidOperationException(xml);

        return result;
    }
}