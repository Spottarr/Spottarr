using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace Spottarr.Web.Modals.Newznab;

public class Capabilities
{
    [XmlElement("server")] public required ServerInfo ServerInfo { get; init; }
    [XmlElement("limits")] public required Limits Limits { get; init; }
    [XmlElement("registration")] public required Registration Registration { get; init; }
    [XmlElement("searching")] public required Searching Searching { get; init; }
    
    [XmlArray("categories"), XmlArrayItem("category")]
    public required Collection<MainCategory> Categories { get; init; }
}