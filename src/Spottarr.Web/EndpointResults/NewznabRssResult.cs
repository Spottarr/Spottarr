using System.Net.Mime;
using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;

namespace Spottarr.Web.EndpointResults;

internal sealed class NewznabRssResult : IResult
{
    private readonly SyndicationFeed _feed;

    private static readonly XmlWriterSettings XmlWriterSettings = new()
    {
        // Disable BOM, it breaks parsing by other ARRs
        Encoding = new UTF8Encoding(false),
        NewLineHandling = NewLineHandling.Entitize,
        Indent = true,
        Async = true,
    };

    public NewznabRssResult(SyndicationFeed feed) => _feed = feed;

    public async Task ExecuteAsync(HttpContext httpContext)
    {
        var ms = new MemoryStream();
        await using var writer = XmlWriter.Create(ms, XmlWriterSettings);

        var rssFormatter = new Rss20FeedFormatter(_feed, false);
        rssFormatter.WriteTo(writer);
        await writer.FlushAsync();

        ms.Position = 0;

        httpContext.Response.ContentType = MediaTypeNames.Application.Xml;
        await ms.CopyToAsync(httpContext.Response.Body, httpContext.RequestAborted);
    }
}