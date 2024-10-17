using System.Xml.Serialization;

namespace Spottarr.Web.Newznab.Models;

public class Search
{
    [XmlAttribute("available")] public required string Available { get; init; }
    [XmlAttribute("supportedParams")] public required string SupportedParams { get; init; }
}