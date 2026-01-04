using System.Xml;
using System.Xml.Serialization;
using Spottarr.Services.Helpers;

namespace Spottarr.Web.Newznab.Models;

internal class Error : IXmlWritable
{
    [XmlAttribute("code")] public required ErrorCode Code { get; init; }
    [XmlAttribute("description")] public required string Description { get; init; }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteAttributeString("code", Code.ToString("D"));
        writer.WriteAttributeString("description", Description);
    }
}