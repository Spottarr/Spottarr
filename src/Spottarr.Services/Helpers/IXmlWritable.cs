using System.Xml;

namespace Spottarr.Services.Helpers;

public interface IXmlWritable
{
    void WriteXml(XmlWriter writer);
}