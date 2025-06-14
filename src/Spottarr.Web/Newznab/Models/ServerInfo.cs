using System.Xml.Serialization;

namespace Spottarr.Web.Newznab.Models;

public sealed class ServerInfo
{
    [XmlAttribute("version")] public string Version { get; set; } = "";
    [XmlAttribute("title")] public string Title { get; set; } = "";
    [XmlAttribute("strapline")] public string Tagline { get; set; } = "";
    [XmlAttribute("email")] public string Email { get; set; } = "";
    [XmlAttribute("url")] public string Host { get; set; } = "";
    [XmlAttribute("image")] public string Image { get; set; } = "";
    [XmlAttribute("type")] public string Type { get; set; } = "";
}