using System.Xml;

namespace Spottarr.Services.Helpers;

public static class XmlWriterExtensions
{
    public static void WriteElement(this XmlWriter writer, string name, IXmlWritable serializable)
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(serializable);

        writer.WriteStartElement(name);
        serializable.WriteXml(writer);
        writer.WriteEndElement();
    }

    public static void WriteCollection(
        this XmlWriter writer,
        string itemName,
        IEnumerable<IXmlWritable> collection
    )
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(collection);

        foreach (var value in collection)
        {
            writer.WriteElement(itemName, value);
        }
    }

    public static void WriteCollection(
        this XmlWriter writer,
        string name,
        string itemName,
        IEnumerable<IXmlWritable> collection
    )
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(collection);

        writer.WriteStartElement(name);
        writer.WriteCollection(itemName, collection);
        writer.WriteEndElement();
    }

    public static void WriteCollection<T>(
        this XmlWriter writer,
        string itemName,
        IEnumerable<T> collection
    )
        where T : unmanaged
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(collection);

        foreach (var value in collection)
        {
            writer.WriteStartElement(itemName);
            writer.WriteValue(value);
            writer.WriteEndElement();
        }
    }

    public static void WriteCollection<T>(
        this XmlWriter writer,
        string name,
        string itemName,
        IEnumerable<T> collection
    )
        where T : unmanaged
    {
        ArgumentNullException.ThrowIfNull(writer);
        ArgumentNullException.ThrowIfNull(collection);

        writer.WriteStartElement(name);
        writer.WriteCollection(itemName, collection);
        writer.WriteEndElement();
    }
}
