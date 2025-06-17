using System.Xml;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace Spottarr.Services.Helpers;

public abstract class XmlReadable : IXmlSerializable
{
    public XmlSchema? GetSchema() => null;
    public abstract void ReadXml(XmlReader reader);
    public void WriteXml(XmlWriter writer) => throw new NotImplementedException();
}