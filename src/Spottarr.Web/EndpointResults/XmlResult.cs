using System.Diagnostics.CodeAnalysis;
using System.Net.Mime;
using System.Text;
using System.Xml.Serialization;

namespace Spottarr.Web.EndpointResults;

internal sealed class XmlResult<T> : IResult
{
    [UnconditionalSuppressMessage("Trimming",
        "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
        Justification = "<Pending>")]
    private static readonly XmlSerializer Serializer = new(typeof(T));

    private readonly T _result;

    public XmlResult(T result) => _result = result;

    [UnconditionalSuppressMessage("Trimming",
        "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
        Justification = "<Pending>")]
    public async Task ExecuteAsync(HttpContext httpContext)
    {
        using var ms = new MemoryStream();

        Serializer.Serialize(ms, _result);
        ms.Position = 0;

        httpContext.Response.ContentType = MediaTypeNames.Application.Xml;
        await ms.CopyToAsync(httpContext.Response.Body);
    }
}
