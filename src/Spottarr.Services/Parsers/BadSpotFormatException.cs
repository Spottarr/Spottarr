namespace Spottarr.Services.Parsers;

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