using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace Spottarr.Services.Spotnet;

public sealed class Category
{
    [XmlText] public string Text { get; set; } = string.Empty;
    [XmlElement(ElementName = "Sub")] public Collection<string> Sub { get; } = [];
}