using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Serialization;
using Spottarr.Services.Helpers;

namespace Spottarr.Web.Newznab.Models;

internal sealed class MainCategory : Category
{
    [XmlElement("subcat")] public required Collection<Category> SubCategories { get; init; }

    public override void WriteXml(XmlWriter writer)
    {
        base.WriteXml(writer);
        writer.WriteCollection("subcat", SubCategories);
    }
}