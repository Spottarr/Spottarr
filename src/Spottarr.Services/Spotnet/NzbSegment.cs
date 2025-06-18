using System.Xml.Serialization;

namespace Spottarr.Services.Spotnet;

public class NzbSegment
{
    [XmlElement(ElementName = "Segment")] public string Segment { get; set; } = string.Empty;
}