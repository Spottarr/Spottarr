using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace Spottarr.Web.Newznab.Models;

public sealed class MainCategory : Category
{
    [XmlElement("subcat")] public required Collection<Category> SubCategories { get; init; }
}