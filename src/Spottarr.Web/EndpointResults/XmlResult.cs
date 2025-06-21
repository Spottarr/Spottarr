using System.Net.Mime;
using System.Xml;
using Spottarr.Services.Helpers;

namespace Spottarr.Web.EndpointResults;

internal sealed class XmlResult<T> : IResult where T : IXmlWritable
{
    private readonly string _rootElement;
    private readonly T _result;

    public XmlResult(string rootElement, T result)
    {
        _rootElement = rootElement;
        _result = result;
    }

    public async Task ExecuteAsync(HttpContext httpContext)
    {
        var ms = new MemoryStream();
        await using var writer = XmlWriter.Create(ms, new XmlWriterSettings
        {
            Indent = true,
            Async = true,
        });

        writer.WriteElement(_rootElement, _result);
        await writer.FlushAsync();

        ms.Position = 0;

        httpContext.Response.ContentType = MediaTypeNames.Application.Xml;
        await ms.CopyToAsync(httpContext.Response.Body);
    }
}