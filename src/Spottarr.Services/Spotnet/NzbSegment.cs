using System.Xml.Serialization;

namespace Spottarr.Services.Spotnet;

public class NzbSegment
{
    [XmlElement(ElementName = "Segment")] public required string Segment { get; init; }
}