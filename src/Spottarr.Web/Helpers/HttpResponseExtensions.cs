using System.Net.Mime;
using System.Text;
using System.Xml;
using Spottarr.Services.Helpers;

namespace Spottarr.Web.Helpers;

internal static class HttpResponseExtensions
{
    public static async Task WriteAsXmlAsync<TResult>(this HttpResponse response, TResult result, string rootElement,
        CancellationToken cancellationToken) where TResult : IXmlWritable
    {
        var ms = new MemoryStream();
        await using var writer = XmlWriter.Create(ms, new XmlWriterSettings
        {
            // Disable BOM, it breaks parsing by other ARRs
            Encoding = new UTF8Encoding(false),
            Indent = true,
            Async = true
        });

        await writer.WriteStartElementAsync(null, rootElement, null);
        await writer.WriteAttributeStringAsync("xmlns", "xsi", null, "http://www.w3.org/2001/XMLSchema-instance");
        await writer.WriteAttributeStringAsync("xmlns", "xsd", null, "http://www.w3.org/2001/XMLSchema");
        result.WriteXml(writer);
        await writer.WriteEndElementAsync();

        await writer.FlushAsync();

        ms.Position = 0;

        response.ContentType = MediaTypeNames.Application.Xml;
        await ms.CopyToAsync(response.Body, cancellationToken);
    }
}