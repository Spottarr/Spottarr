using System.Xml;
using System.Xml.Serialization;
using Spottarr.Services.Helpers;

namespace Spottarr.Web.Newznab.Models;

internal sealed class Search : XmlWritable
{
    [XmlAttribute("available")] public required string Available { get; init; }
    [XmlAttribute("supportedParams")] public required string SupportedParams { get; init; }

    public override void WriteXml(XmlWriter writer)
    {
        writer.WriteAttributeString("available", Available);
        writer.WriteAttributeString("supportedParams", SupportedParams);
    }
}