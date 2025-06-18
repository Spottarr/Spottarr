using System.Xml.Serialization;

namespace Spottarr.Services.Spotnet;

public sealed class Posting
{
    [XmlElement(ElementName = "Key")] public int Key { get; set; }
    [XmlElement(ElementName = "Created")] public int Created { get; set; }
    [XmlElement(ElementName = "Poster")] public string Poster { get; set; } = string.Empty;
    [XmlElement(ElementName = "Title")] public string Title { get; set; } = string.Empty;

    [XmlElement(ElementName = "Description")]
    public string Description { get; set; } = string.Empty;

    [XmlElement(ElementName = "Tag")] public string Tag { get; set; } = string.Empty;
    [XmlElement(ElementName = "Website")] public string Website { get; set; } = string.Empty;
    [XmlElement(ElementName = "Image")] public ImageSegment Image { get; set; } = new();
    [XmlElement(ElementName = "Size")] public long Size { get; set; }
    [XmlElement(ElementName = "Category")] public Category Category { get; set; } = new();
    [XmlElement(ElementName = "NZB")] public NzbSegment Nzb { get; set; } = new();

    /* Optional fields
     not listed on https://github.com/spotnet/spotnet/wiki/Spot-Xml-format
     but present in https://github.com/spotweb/spotweb/blob/develop/lib/services/Format/Services_Format_Parsing.php */
    [XmlElement(ElementName = "Filename")] public string Filename { get; set; } = string.Empty;

    [XmlElement(ElementName = "Newsgroup")]
    public string Newsgroup { get; set; } = string.Empty;
}