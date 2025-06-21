using System.Xml;
using System.Xml.Serialization;
using Spottarr.Services.Helpers;

namespace Spottarr.Web.Newznab.Models;

internal sealed class ServerInfo : IXmlWritable
{
    [XmlAttribute("version")] public required string Version { get; init; }
    [XmlAttribute("title")] public required string Title { get; init; }
    [XmlAttribute("strapline")] public required string Tagline { get; init; }
    [XmlAttribute("email")] public required string Email { get; init; }
    [XmlAttribute("url")] public required string Host { get; init; }
    [XmlAttribute("image")] public required string Image { get; init; }
    [XmlAttribute("type")] public required string Type { get; init; }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteAttributeString("version", Version);
        writer.WriteAttributeString("title", Title);
        writer.WriteAttributeString("strapline", Tagline);
        writer.WriteAttributeString("email", Email);
        writer.WriteAttributeString("url", Host);
        writer.WriteAttributeString("image", Image);
        writer.WriteAttributeString("type", Type);
    }
}