using System.Xml;
using Spottarr.Services.Helpers;
using Spottarr.Services.Spotnet;

namespace Spottarr.Services.Parsers;

internal class SpotnetXmlParser
{
    private static readonly XmlReaderSettings XmlReaderSettings = new()
    {
        Async = true,
        // Spot XML headers often contain invalid characters
        CheckCharacters = false
    };

    public static async Task<SpotnetXml> Parse(string xml)
    {
        using var reader = new StringReader(xml);
        return await Parse(reader);
    }

    public static async Task<SpotnetXml> Parse(IEnumerable<string> xml)
    {
        using var reader = new StringEnumerableReader(xml);
        return await Parse(reader);
    }

    private static async Task<SpotnetXml> Parse(TextReader textReader)
    {
        using var reader = XmlReader.Create(textReader, XmlReaderSettings);

        reader.ReadStartElement("Spotnet");
        reader.ReadStartElement("Posting");

        var result = new SpotnetXml();

        while (reader.NodeType != XmlNodeType.EndElement || reader.Name != "Posting")
        {
            if (reader.NodeType == XmlNodeType.Element)
            {
                switch (reader.Name)
                {
                    case "Key":
                        result.Posting.Key = reader.ReadElementContentAsInt();
                        break;
                    case "Created":
                        result.Posting.Created = reader.ReadElementContentAsInt();
                        break;
                    case "Poster":
                        result.Posting.Poster = await reader.ReadElementContentAsStringAsync();
                        break;
                    case "Title":
                        result.Posting.Title = await reader.ReadElementContentAsStringAsync();
                        break;
                    case "Description":
                        result.Posting.Description = await reader.ReadElementContentAsStringAsync();
                        break;
                    case "Image":
                        if (int.TryParse(reader.GetAttribute("Width"), out var width))
                            result.Posting.Image.Width = width;
                        if (int.TryParse(reader.GetAttribute("Height"), out var height))
                            result.Posting.Image.Height = height;

                        reader.ReadStartElement("Image");
                        if (reader.IsStartElement("Segment"))
                            result.Posting.Image.Segment = await reader.ReadElementContentAsStringAsync();
                        reader.ReadEndElement();
                        break;
                    case "Size":
                        result.Posting.Size = reader.ReadElementContentAsLong();
                        break;
                    case "Category":
                        reader.ReadStartElement("Category");
                        result.Posting.Category.Text = await reader.ReadContentAsStringAsync();
                        while (reader.IsStartElement("Sub"))
                        {
                            var sub = await reader.ReadElementContentAsStringAsync();
                            result.Posting.Category.Sub.Add(sub);
                        }

                        reader.ReadEndElement();
                        break;
                    case "NZB":
                        reader.ReadStartElement("NZB");
                        if (reader.IsStartElement("Segment"))
                            result.Posting.Nzb.Segment = await reader.ReadElementContentAsStringAsync();
                        reader.ReadEndElement();
                        break;
                    default:
                        await reader.SkipAsync(); // Skip unknown elements
                        break;
                }
            }
            else
            {
                await reader.ReadAsync(); // Move to the next node
            }
        }

        reader.ReadEndElement(); // End Posting
        reader.ReadEndElement(); // End Spotnet

        return result;
    }
}