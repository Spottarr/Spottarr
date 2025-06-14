using System.Xml.Serialization;

namespace Spottarr.Web.Newznab.Models;

public sealed class Search
{
    [XmlAttribute("available")] public string Available { get; set; } = "";
    [XmlAttribute("supportedParams")] public string SupportedParams { get; set; } = "";
}