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
            // Disable BOM, it breaks parsing by other ARRs
            Encoding = new UTF8Encoding(false),
            Indent = true,
            Async = true
        });

        await writer.WriteStartElementAsync(null, _rootElement, null);
        await writer.WriteAttributeStringAsync("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
        await writer.WriteAttributeStringAsync("xmlns", "xsd", null, "http://www.w3.org/2001/XMLSchema");
        _result.WriteXml(writer);
        await writer.WriteEndElementAsync();

        await writer.FlushAsync();

        ms.Position = 0;

        httpContext.Response.ContentType = MediaTypeNames.Application.Xml;
        await ms.CopyToAsync(httpContext.Response.Body);
    }
}
