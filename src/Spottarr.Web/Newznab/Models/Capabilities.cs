using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace Spottarr.Web.Newznab.Models;

[XmlRoot("caps")]
public sealed class Capabilities
{
    [XmlElement("server")] public ServerInfo ServerInfo { get; set; } = null!;
    [XmlElement("limits")] public Limits Limits { get; set; } = null!;
    [XmlElement("registration")] public Registration Registration { get; set; } = null!;
    [XmlElement("searching")] public Searching Searching { get; set; } = null!;

    [XmlArray("categories"), XmlArrayItem("category")]
    public Collection<MainCategory> Categories { get; } = [];
}

[XmlSerializable(typeof(Capabilities))]
internal static partial class XmlSerializers;