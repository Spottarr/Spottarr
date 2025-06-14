using System.Xml.Serialization;

namespace Spottarr.Web.Newznab.Models;

public class Category
{
    [XmlAttribute("id")] public int Id { get; set; }
    [XmlAttribute("name")] public string Name { get; set; } = "";
}