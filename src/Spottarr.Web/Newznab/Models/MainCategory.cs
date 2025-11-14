using System.Collections.ObjectModel;
using System.Globalization;
using System.Xml;
using System.Xml.Serialization;
using Spottarr.Services.Helpers;

namespace Spottarr.Web.Newznab.Models;

internal sealed class MainCategory : IXmlWritable
{
    [XmlAttribute("id")] public int Id { get; init; }
    [XmlAttribute("name")] public required string Name { get; init; }
    [XmlElement("subcat")] public required Collection<Category> SubCategories { get; init; }

    public void WriteXml(XmlWriter writer)
    {
        writer.WriteAttributeString("id", Id.ToString(CultureInfo.InvariantCulture));
        writer.WriteAttributeString("name", Name);
        writer.WriteCollection("subcat", SubCategories);
    }
}