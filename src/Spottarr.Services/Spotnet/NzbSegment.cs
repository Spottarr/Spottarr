using System.Xml.Serialization;

namespace Spottarr.Services.Nntp;

public class NzbSegment
{
    [XmlElement(ElementName = "Segment")] public required string Segment { get; init; }
}