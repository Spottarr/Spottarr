using System.IO.Compression;
using Usenet.Util;

namespace Spottarr.Services.Parsers;

internal static class NzbArticleParser
{
    public static async Task<MemoryStream> Parse(string body)
    {
        body = body
            .Replace("=A", "\0", StringComparison.Ordinal)
            .Replace("=B", "\r", StringComparison.Ordinal)
            .Replace("=C", "\n", StringComparison.Ordinal)
            .Replace("=D", "=", StringComparison.Ordinal);
        
        using var msIn = new MemoryStream(UsenetEncoding.Default.GetBytes(body));
        await using var ds = new DeflateStream(msIn, CompressionMode.Decompress);
        
        var msOut = new MemoryStream();
        await ds.CopyToAsync(msOut);
        msOut.Position = 0;

        return msOut;
    }
}