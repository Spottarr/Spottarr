using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace Spottarr.Services.Spotnet;

public sealed class Category
{
    [XmlText] public string Text { get; set; } = "";
    [XmlElement(ElementName = "Sub")] public Collection<string> Sub { get; } = [];
}