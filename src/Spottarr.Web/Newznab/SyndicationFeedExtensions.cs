using System.ServiceModel.Syndication;
using System.Xml;
using System.Xml.Linq;

namespace Spottarr.Web.Newznab;

internal static class SyndicationFeedExtensions
{
    private static readonly XNamespace Namespace = XNamespace.Get("https://www.newznab.com/DTD/2010/feeds/attributes/");

    public static SyndicationItem AddNewznabAttribute(this SyndicationItem item, string name, string? value)
    {
        if(value == null) return item;
        
        item.ElementExtensions.Add(new XElement(Namespace.GetName("attr"), new XAttribute("name", name),
            new XAttribute("value", value)));

        return item;
    }
    
    public static SyndicationItem AddNewznabNzbUrl(this SyndicationItem item, Uri url, long bytes)
    {
        item.Links.Add(SyndicationLink.CreateMediaEnclosureLink(url, "application/x-nzb", bytes));
        return item;
    }
    
    public static SyndicationItem AddCategory(this SyndicationItem item, string name)
    {
        item.Categories.Add(new SyndicationCategory(name));
        
        return item;
    }

    public static SyndicationFeed AddNewznabNamespace(this SyndicationFeed feed)
    {
        feed.AttributeExtensions.Add(new XmlQualifiedName("newznab", XNamespace.Xmlns.NamespaceName),
            Namespace.NamespaceName);

        return feed;
    }
    
    public static SyndicationFeed AddNewznabResponseInfo(this SyndicationFeed feed, int offset, int total)
    {
        feed.ElementExtensions.Add(new XElement(Namespace.GetName("response"), new XAttribute("offset", offset),
            new XAttribute("total", total)));

        return feed;
    }
}