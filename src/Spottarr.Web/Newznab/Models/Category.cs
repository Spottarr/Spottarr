using System.Globalization;
using System.Xml;
using System.Xml.Serialization;
using Spottarr.Services.Helpers;

namespace Spottarr.Web.Newznab.Models;

internal class Category : XmlWritable
{
    [XmlAttribute("id")] public int Id { get; init; }
    [XmlAttribute("name")] public required string Name { get; init; }

    public override void WriteXml(XmlWriter writer)
    {
        writer.WriteAttributeString("id", Id.ToString(CultureInfo.InvariantCulture));
        writer.WriteAttributeString("name", Name);
    }
}