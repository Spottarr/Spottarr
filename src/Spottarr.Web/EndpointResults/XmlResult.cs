using Spottarr.Services.Helpers;
using Spottarr.Web.Helpers;

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

    public Task ExecuteAsync(HttpContext httpContext) =>
        httpContext.Response.WriteAsXmlAsync(_result, _rootElement, httpContext.RequestAborted);
}