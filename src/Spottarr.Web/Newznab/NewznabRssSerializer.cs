using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;

namespace Spottarr.Web.Newznab;

internal static class NewznabRssSerializer
{
    private static readonly XmlWriterSettings XmlWriterSettings = new()
    {
        // Disable BOM, it breaks parsing by other ARRs
        Encoding = new UTF8Encoding(false),
        NewLineHandling = NewLineHandling.Entitize,
        Indent = true,
        Async = true,
    };
    
    public static MemoryStream Serialize(SyndicationFeed feed)
    {
        var ms = new MemoryStream();
        using var writer = XmlWriter.Create(ms, XmlWriterSettings);
        
        var rssFormatter = new Rss20FeedFormatter(feed, false);
        rssFormatter.WriteTo(writer);
        writer.Flush();

        ms.Position = 0;

        return ms;
    }
}