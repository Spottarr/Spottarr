using System.Xml.Serialization;

namespace Spottarr.Web.Newznab.Models;

public sealed class Registration
{
    [XmlAttribute("available")] public required string Available { get; init; }
    [XmlAttribute("open")] public required string Open { get; init; }
}