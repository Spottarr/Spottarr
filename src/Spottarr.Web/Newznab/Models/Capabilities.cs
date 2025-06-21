using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Serialization;
using Spottarr.Services.Helpers;

namespace Spottarr.Web.Newznab.Models;

[XmlRoot("caps")]
internal sealed class Capabilities : IXmlWritable
{
    [XmlElement("server")] public required ServerInfo ServerInfo { get; init; }
    [XmlElement("limits")] public required Limits Limits { get; init; }
    [XmlElement("registration")] public required Registration Registration { get; init; }
    [XmlElement("searching")] public required Searching Searching { get; init; }

    [XmlArray("categories"), XmlArrayItem("category")]
    public required Collection<MainCategory> Categories { get; init; }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteElement("server", ServerInfo);
        writer.WriteElement("limits", Limits);
        writer.WriteElement("registration", Registration);
        writer.WriteElement("searching", Searching);
        writer.WriteCollection("categories", "category", Categories);
    }
}