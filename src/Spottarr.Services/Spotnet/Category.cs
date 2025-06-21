using System.Collections.ObjectModel;
using System.Xml;
using System.Xml.Serialization;
using Spottarr.Services.Helpers;

namespace Spottarr.Services.Spotnet;

public sealed class Category : IXmlReadable<Category>
{
    [XmlText] public string Text { get; set; } = string.Empty;
    [XmlElement(ElementName = "Sub")] public Collection<string> Sub { get; } = [];

    public static async Task<Category> ReadXml(XmlReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);

        var result = new Category();
        var depth = reader.Depth;

        result.Text = await reader.ReadContentAsStringAsync();
        while (reader.Depth >= depth)
        {
            if (reader.NodeType != XmlNodeType.Element)
            {
                await reader.ReadAsync();
                continue;
            }

            switch (reader.Name)
            {
                case "Sub":
                    var sub = await reader.ReadElementContentAsStringAsync();
                    result.Sub.Add(sub);
                    break;
                default:
                    await reader.SkipAsync(); // Skip unknown elements
                    break;
            }
        }

        return result;
    }
}