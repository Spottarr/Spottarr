using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace Spottarr.Services.Spotnet;

public sealed class Category
{
    [XmlText] public required string Text { get; init; }
    [XmlElement(ElementName = "Sub")] public required Collection<string> Sub { get; init; }
}