using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace Spottarr.Web.Newznab.Models;

[XmlRoot("caps")]
public sealed class Capabilities
{
    [XmlElement("server")] public required ServerInfo ServerInfo { get; init; }
    [XmlElement("limits")] public required Limits Limits { get; init; }
    [XmlElement("registration")] public required Registration Registration { get; init; }
    [XmlElement("searching")] public required Searching Searching { get; init; }
    
    [XmlArray("categories"), XmlArrayItem("category")]
    public required Collection<MainCategory> Categories { get; init; }
}