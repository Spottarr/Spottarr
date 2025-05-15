using System.Globalization;
using System.ServiceModel.Syndication;
using System.Xml;
using System.Xml.Linq;
using Spottarr.Data.Entities.Enums;
using Spottarr.Web.Helpers;

namespace Spottarr.Web.Newznab;

internal static class SyndicationFeedExtensions
{
    private static readonly XNamespace Namespace = XNamespace.Get("https://www.newznab.com/DTD/2010/feeds/attributes/");

    public static SyndicationItem AddElement(this SyndicationItem item, string name, string? value)
    {
        if (value == null) return item;

        item.ElementExtensions.Add(new XElement(name, value.SanitizeXmlString()));

        return item;
    }

    public static SyndicationItem AddNewznabAttribute(this SyndicationItem item, string name, string? value)
    {
        if (value == null) return item;

        item.ElementExtensions.Add(new XElement(Namespace.GetName("attr"), new XAttribute("name", name),
            new XAttribute("value", value.SanitizeXmlString())));

        return item;
    }

    public static SyndicationItem AddNewznabAttributes(this SyndicationItem item, string name,
        IEnumerable<string> values)
    {
        foreach (var value in values)
        {
            item.AddNewznabAttribute(name, value);
        }

        return item;
    }

    public static SyndicationItem AddNewznabNzbUrl(this SyndicationItem item, Uri url, long bytes)
    {
        item.Links.Add(SyndicationLink.CreateMediaEnclosureLink(url, "application/x-nzb", bytes));
        return item;
    }

    public static SyndicationItem AddCategories(this SyndicationItem item, IEnumerable<NewznabCategory> categories)
    {
        foreach (int category in categories)
        {
            item.Categories.Add(new SyndicationCategory(category.ToString(CultureInfo.InvariantCulture)));
        }

        return item;
    }

    public static SyndicationItem AddPublishDate(this SyndicationItem item, DateTimeOffset date)
    {
        // Force RFC 1123 date formats for the pubDate property.
        // When using PublishDate the date is formatted with Z instead of GMT for the zone specifier.
        // This is not accepted by some clients.
        item.AddElement("pubDate", date.ToUniversalTime().ToString("r"));

        // item.PublishDate = date;
        return item;
    }

    public static SyndicationFeed AddLogo(this SyndicationFeed feed, Uri uri)
    {
        feed.ImageUrl = uri;
        return feed;
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