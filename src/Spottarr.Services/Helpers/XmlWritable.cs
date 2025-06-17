using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Spottarr.Services.Helpers;

public abstract class XmlWritable : IXmlSerializable
{
    public XmlSchema? GetSchema() => null;
    public void ReadXml(XmlReader reader) => throw new NotImplementedException();
    public abstract void WriteXml(XmlWriter writer);
}