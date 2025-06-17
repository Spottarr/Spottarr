using System.Xml;
using System.Xml.Serialization;

namespace Spottarr.Services.Helpers;

public static class XmlWriterExtensions
{
    public static void WriteElement(this XmlWriter writer, string name, IXmlSerializable serializable)
    {
        ArgumentNullException.ThrowIfNull(writer, nameof(writer));
        ArgumentNullException.ThrowIfNull(serializable, nameof(serializable));

        writer.WriteStartElement(name);
        serializable.WriteXml(writer);
        writer.WriteEndElement();
    }

    public static void WriteCollection(this XmlWriter writer, string itemName, IEnumerable<IXmlSerializable> collection)
    {
        ArgumentNullException.ThrowIfNull(writer, nameof(writer));
        ArgumentNullException.ThrowIfNull(collection, nameof(collection));

        foreach (var value in collection)
        {
            writer.WriteElement(itemName, value);
        }
    }

    public static void WriteCollection(this XmlWriter writer, string name, string itemName,
        IEnumerable<IXmlSerializable> collection)
    {
        ArgumentNullException.ThrowIfNull(writer, nameof(writer));
        ArgumentNullException.ThrowIfNull(collection, nameof(collection));

        writer.WriteStartElement(name);
        writer.WriteCollection(itemName, collection);
        writer.WriteEndElement();
    }

    public static void WriteCollection<T>(this XmlWriter writer, string itemName, IEnumerable<T> collection)
        where T : unmanaged
    {
        ArgumentNullException.ThrowIfNull(writer, nameof(writer));
        ArgumentNullException.ThrowIfNull(collection, nameof(collection));

        foreach (var value in collection)
        {
            writer.WriteStartElement(itemName);
            writer.WriteValue(value);
            writer.WriteEndElement();
        }
    }

    public static void WriteCollection<T>(this XmlWriter writer, string name, string itemName,
        IEnumerable<T> collection) where T : unmanaged
    {
        ArgumentNullException.ThrowIfNull(writer, nameof(writer));
        ArgumentNullException.ThrowIfNull(collection, nameof(collection));

        writer.WriteStartElement(name);
        writer.WriteCollection(itemName, collection);
        writer.WriteEndElement();
    }
}