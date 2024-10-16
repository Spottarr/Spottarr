using System.ServiceModel.Syndication;
using System.Xml;
using System.Xml.Linq;

namespace Spottarr.Web.Helpers;

internal static class SyndicationFeedExtensions
{
    private static readonly XNamespace Namespace = XNamespace.Get("https://www.newznab.com/DTD/2010/feeds/attributes/");

    public static SyndicationItem AddNewznabAttribute(this SyndicationItem item, string name, string value)
    {
        item.ElementExtensions.Add(new XElement(Namespace.GetName("attr"), new XAttribute("name", name),
            new XAttribute("value", value)));

        return item;
    }

    public static SyndicationFeed AddNewznabNamespace(this SyndicationFeed feed)
    {
        feed.AttributeExtensions.Add(new XmlQualifiedName("newznab", XNamespace.Xmlns.NamespaceName),
            Namespace.NamespaceName);

        return feed;
    }
}