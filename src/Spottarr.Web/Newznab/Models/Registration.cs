using System.Xml.Serialization;

namespace Spottarr.Web.Newznab.Models;

public sealed class Registration
{
    [XmlAttribute("available")] public string Available { get; set; } = "";
    [XmlAttribute("open")] public string Open { get; set; } = "";
}