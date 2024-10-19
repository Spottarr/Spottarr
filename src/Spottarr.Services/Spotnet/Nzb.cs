using System.Xml.Serialization;

namespace Spottarr.Services.Nntp;

internal class Nzb
{
    [XmlElement(ElementName = "Segment")] public required string Segment { get; init; }
}