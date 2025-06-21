using System.Xml;
using System.Xml.Serialization;
using Spottarr.Services.Helpers;

namespace Spottarr.Web.Newznab.Models;

internal sealed class Registration : IXmlWritable
{
    [XmlAttribute("available")] public required string Available { get; init; }
    [XmlAttribute("open")] public required string Open { get; init; }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteAttributeString("available", Available);
        writer.WriteAttributeString("open", Open);
    }
}