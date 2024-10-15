using System.Xml.Serialization;

namespace Spottarr.Web.Modals.Newznab;

public class Registration
{
    [XmlAttribute("available")] public required string Available { get; init; }
    [XmlAttribute("open")] public required string Open { get; init; }
}