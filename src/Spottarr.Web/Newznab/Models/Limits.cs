using System.Xml.Serialization;

namespace Spottarr.Web.Newznab.Models;

public sealed class Limits
{
    [XmlAttribute("max")] public int Max { get; set; }
    [XmlAttribute("default")] public int Default { get; set; }
}