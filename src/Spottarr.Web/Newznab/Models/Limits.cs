using System.Globalization;
using System.Xml;
using System.Xml.Serialization;
using Spottarr.Services.Helpers;

namespace Spottarr.Web.Newznab.Models;

internal sealed class Limits : IXmlWritable
{
    [XmlAttribute("max")] public int Max { get; init; }
    [XmlAttribute("default")] public int Default { get; init; }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteAttributeString("max", Max.ToString(CultureInfo.InvariantCulture));
        writer.WriteAttributeString("default", Default.ToString(CultureInfo.InvariantCulture));
    }
}