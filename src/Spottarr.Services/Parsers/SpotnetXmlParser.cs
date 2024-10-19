using System.Xml;
using System.Xml.Serialization;
using Spottarr.Services.Nntp;

namespace Spottarr.Services;

internal class SpotnetXmlParser
{
    private static readonly XmlSerializer Serializer = new(typeof(Spotnet)); 
    private static readonly XmlReaderSettings XmlREaderSettings = new()
    {
        Async = true,
    };
    
    public static Spotnet Parse(string xml)
    {
        Spotnet? result;
        
        try
        {
            using var stringReader = new StringReader(xml);
            using var xmlReader = XmlReader.Create(stringReader, XmlREaderSettings);
            result = Serializer.Deserialize(xmlReader) as Spotnet;
        }
        catch (Exception ex)
        {
            throw new BadSpotFormatException(xml, ex);
        }

        if (result == null) throw new BadSpotFormatException(xml);

        return result;
    }
}

public class BadSpotFormatException : Exception
{
    public string Xml { get; }
    
    public BadSpotFormatException() : base("Spot XML is not in the correct format")
    {
        Xml = string.Empty;
    }
    
    public BadSpotFormatException(string xml) : base("Spot XML is not in the correct format")
    {
        Xml = xml;
    }

    public BadSpotFormatException(string xml, Exception innerException) : base("Spot XML is not in the correct format", innerException)
    {
        Xml = xml;
    }
}