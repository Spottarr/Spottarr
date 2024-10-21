using System.Xml.Serialization;

namespace Spottarr.Services.Spotnet;

[XmlRoot(ElementName = "Spotnet")]
public sealed class SpotnetXml
{
    public const string HeaderName = "X-XML";
    
    [XmlElement(ElementName = "Posting")] public required Posting Posting { get; init; }
}