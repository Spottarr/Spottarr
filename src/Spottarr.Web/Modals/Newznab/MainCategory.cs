using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace Spottarr.Web.Modals.Newznab;

public class MainCategory : Category
{
    [XmlElement("subcat")] public required Collection<Category> SubCategories { get; init; }
}