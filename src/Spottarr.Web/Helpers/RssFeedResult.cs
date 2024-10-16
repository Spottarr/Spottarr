using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using Microsoft.AspNetCore.Mvc;

namespace Spottarr.Web.Helpers;

internal sealed class RssFeedResult : ActionResult
{
    private const string RssContentType = "application/rss+xml; charset=utf-8";
    
    private static readonly XmlWriterSettings XmlWriterSettings = new()
    {
        Encoding = Encoding.UTF8,
        NewLineHandling = NewLineHandling.Entitize,
        NewLineOnAttributes = true,
        Indent = true,
        Async = true,
    };

    private readonly SyndicationFeed _feed;

    public RssFeedResult(SyndicationFeed feed) => _feed = feed;

    public override async Task ExecuteResultAsync(ActionContext context)
    {
        var response = context.HttpContext.Response;
        
        SetHeaders(response);
        var ms = SerializeFeed();
        
        await ms.CopyToAsync(response.Body);

        await base.ExecuteResultAsync(context);
    }

    public override void ExecuteResult(ActionContext context)
    {
        var response = context.HttpContext.Response;

        SetHeaders(response);
        var ms = SerializeFeed();
        
        ms.CopyTo(response.Body);
        
        base.ExecuteResult(context);
    }

    private static void SetHeaders(HttpResponse response)
    {
        response.ContentType = RssContentType;
        response.StatusCode = StatusCodes.Status200OK;
    }

    private MemoryStream SerializeFeed()
    {
        var ms = new MemoryStream();
        using var writer = XmlWriter.Create(ms, XmlWriterSettings);
        
        var rssFormatter = new Rss20FeedFormatter(_feed, false);
        rssFormatter.WriteTo(writer);

        return ms;
    }
}