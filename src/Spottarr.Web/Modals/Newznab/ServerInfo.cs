using System.Xml.Serialization;

namespace Spottarr.Web.Modals.Newznab;

public class ServerInfo
{
    [XmlAttribute("name")]
    public required string Name { get; init; }
}