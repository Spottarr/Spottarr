using System.IO.Compression;
using Spottarr.Data.Entities;
using Usenet.Util;

namespace Spottarr.Services.Parsers;

internal static class NzbArticleParser
{
    public static async Task<NzbFile> Parse(string messageId, string body)
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
        
        var now = DateTimeOffset.Now.UtcDateTime;
        
        return new NzbFile
        {
            MessageId = messageId,
            Data = msOut.ToArray(),
            CreatedAt = now,
            UpdatedAt = now
        };
    }
}