using System.Xml;

namespace Spottarr.Services.Helpers;

internal interface IXmlReadable<T>
{
    static abstract Task<T> ReadXml(XmlReader reader);
}