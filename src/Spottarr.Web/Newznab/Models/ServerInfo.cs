using System.Xml.Serialization;

namespace Spottarr.Web.Newznab.Models;

public class ServerInfo
{
    [XmlAttribute("version")] public required string Version { get; init; }
    [XmlAttribute("title")] public required string Title { get; init; }
    [XmlAttribute("strapline")] public required string Tagline { get; init; }
    [XmlAttribute("email")] public required string Email { get; init; }
    [XmlAttribute("url")] public required string Host { get; init; }
    [XmlAttribute("image")] public required string Image { get; init; }
    [XmlAttribute("type")] public required string Type { get; init; }
}