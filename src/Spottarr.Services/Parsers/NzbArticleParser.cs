using System.IO.Compression;
using Usenet.Util;

namespace Spottarr.Services.Parsers;

internal static class NzbArticleParser
{
    public static async Task<string> Parse(string body)
    {
        body = body
            .Replace("=A", "\0", StringComparison.Ordinal)
            .Replace("=B", "\r", StringComparison.Ordinal)
            .Replace("=C", "\n", StringComparison.Ordinal)
            .Replace("=D", "=", StringComparison.Ordinal);
        
        using var ms = new MemoryStream(UsenetEncoding.Default.GetBytes(body));
        await using var ds = new DeflateStream(ms, CompressionMode.Decompress);
        using var sr = new StreamReader(ds);
        
        return await sr.ReadToEndAsync();
    }
}