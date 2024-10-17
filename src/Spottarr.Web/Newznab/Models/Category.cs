using System.Xml.Serialization;

namespace Spottarr.Web.Newznab.Models;

public class Category
{
    [XmlAttribute("id")] public int Id { get; init; }
    [XmlAttribute("name")] public required string Name { get; init; }
}