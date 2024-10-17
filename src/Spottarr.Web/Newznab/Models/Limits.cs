using System.Xml.Serialization;

namespace Spottarr.Web.Newznab.Models;

public class Limits
{
    [XmlAttribute("max")] public int Max { get; init; }
    [XmlAttribute("default")] public int Default { get; init; }
}