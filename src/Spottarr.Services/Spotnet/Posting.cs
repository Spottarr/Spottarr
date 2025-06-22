using System.Xml;
using System.Xml.Serialization;
using Spottarr.Services.Helpers;

namespace Spottarr.Services.Spotnet;

public sealed class Posting : IXmlReadable<Posting>
{
    [XmlElement(ElementName = "Key")] public int Key { get; set; }
    [XmlElement(ElementName = "Created")] public int Created { get; set; }
    [XmlElement(ElementName = "Poster")] public string Poster { get; set; } = string.Empty;
    [XmlElement(ElementName = "Title")] public string Title { get; set; } = string.Empty;

    [XmlElement(ElementName = "Description")]
    public string Description { get; set; } = string.Empty;

    [XmlElement(ElementName = "Tag")] public string Tag { get; set; } = string.Empty;
    [XmlElement(ElementName = "Website")] public string Website { get; set; } = string.Empty;
    [XmlElement(ElementName = "Image")] public ImageSegment Image { get; set; } = null!;
    [XmlElement(ElementName = "Size")] public long Size { get; set; }
    [XmlElement(ElementName = "Category")] public Category Category { get; set; } = null!;
    [XmlElement(ElementName = "NZB")] public NzbSegment Nzb { get; set; } = null!;

    /* Optional fields
     not listed on https://github.com/spotnet/spotnet/wiki/Spot-Xml-format
     but present in https://github.com/spotweb/spotweb/blob/develop/lib/services/Format/Services_Format_Parsing.php */
    [XmlElement(ElementName = "Filename")] public string Filename { get; set; } = string.Empty;

    [XmlElement(ElementName = "Newsgroup")]
    public string Newsgroup { get; set; } = string.Empty;

    public static async Task<Posting> ReadXml(XmlReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);

        var result = new Posting();
        var depth = reader.Depth;

        // XML reader uses a depth-first search.
        // Since the XML elements are not guaranteed to be in order, so we need to loop
        while (reader.Depth >= depth)
        {
            if (reader.NodeType != XmlNodeType.Element)
            {
                await reader.ReadAsync();
                continue;
            }

            switch (reader.Name)
            {
                case "Key":
                    result.Key = reader.ReadElementContentAsInt();
                    break;
                case "Created":
                    result.Created = reader.ReadElementContentAsInt();
                    break;
                case "Poster":
                    result.Poster = await reader.ReadElementContentAsStringAsync();
                    break;
                case "Title":
                    result.Title = await reader.ReadElementContentAsStringAsync();
                    break;
                case "Description":
                    result.Description = await reader.ReadElementContentAsStringAsync();
                    break;
                case "Filename":
                    result.Filename = await reader.ReadElementContentAsStringAsync();
                    break;
                case "Newsgroup":
                    result.Newsgroup = await reader.ReadElementContentAsStringAsync();
                    break;
                case "Image":
                    reader.ReadStartElement("Image");
                    result.Image = await ImageSegment.ReadXml(reader);
                    reader.ReadEndElement();
                    break;
                case "Size":
                    result.Size = reader.ReadElementContentAsLong();
                    break;
                case "Category":
                    reader.ReadStartElement("Category");
                    result.Category = await Category.ReadXml(reader);
                    reader.ReadEndElement();
                    break;
                case "NZB":
                    reader.ReadStartElement("NZB");
                    result.Nzb = await NzbSegment.ReadXml(reader);
                    reader.ReadEndElement();
                    break;
                default:
                    await reader.SkipAsync(); // Skip unknown elements
                    break;
            }
        }

        return result;
    }
}