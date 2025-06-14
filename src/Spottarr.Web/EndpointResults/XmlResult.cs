using System.Net.Mime;
using System.Xml.Serialization;

namespace Spottarr.Web.EndpointResults;

internal sealed class XmlResult<T> : IResult
{
    private readonly T _result;
    private readonly XmlSerializer _serializer;

    public XmlResult(T result, XmlSerializer serializer)
    {
        _result = result;
        _serializer = serializer;
    }

    public async Task ExecuteAsync(HttpContext httpContext)
    {
        using var ms = new MemoryStream();

        _serializer.Serialize(ms, _result);
        ms.Position = 0;

        httpContext.Response.ContentType = MediaTypeNames.Application.Xml;
        await ms.CopyToAsync(httpContext.Response.Body);
    }
}