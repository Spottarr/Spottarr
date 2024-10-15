using System.Xml.Serialization;

namespace Spottarr.Web.Modals.Newznab;

public class Search
{
    [XmlAttribute("available")] public required string Available { get; init; }
    [XmlAttribute("supportedParams")] public required string SupportedParams { get; init; }
}