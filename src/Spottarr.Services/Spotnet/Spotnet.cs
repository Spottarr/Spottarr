using System.Xml.Serialization;

namespace Spottarr.Services.Nntp;

[XmlRoot(ElementName = "Spotnet")]
public sealed class Spotnet
{
    public const string HeaderName = "X-XML";
    
    [XmlElement(ElementName = "Posting")] public required Posting Posting { get; init; }
}