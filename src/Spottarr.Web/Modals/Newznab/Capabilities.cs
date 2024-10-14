using System.Xml.Serialization;

namespace Spottarr.Web.Modals.Newznab;

public class Capabilities
{
    [XmlElement("server")]
    public required ServerInfo Server { get; init; }
}