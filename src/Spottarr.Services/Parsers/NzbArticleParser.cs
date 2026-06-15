using System.Buffers;
using System.IO.Compression;

namespace Spottarr.Services.Parsers;

internal static class NzbArticleParser
{
    /// <summary>
    /// Decodes a Spotnet NZB attachment body. The payload is DEFLATE-compressed and posted with a
    /// small custom escape scheme so the compressed bytes survive transmission as text:
    /// <c>=A</c> → <c>0x00</c>, <c>=B</c> → CR, <c>=C</c> → LF, <c>=D</c> → <c>'='</c>. The article
    /// body also carries the line framing the server added; those CRLFs are not part of the payload
    /// (real CR/LF bytes arrive escaped) and are dropped while unescaping, straight off the pooled
    /// response buffer without round-tripping through a string.
    /// </summary>
    public static async Task<MemoryStream> Parse(
        ReadOnlyMemory<byte> body,
        CancellationToken cancellationToken
    )
    {
        var rented = ArrayPool<byte>.Shared.Rent(body.Length);
        try
        {
            var length = Unescape(body.Span, rented);

            using var compressed = new MemoryStream(rented, 0, length, writable: false);
            await using var deflate = new DeflateStream(compressed, CompressionMode.Decompress);

            var output = new MemoryStream();
            await deflate.CopyToAsync(output, cancellationToken);
            output.Position = 0;

            return output;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(rented);
        }
    }

    private static int Unescape(ReadOnlySpan<byte> body, Span<byte> destination)
    {
        var written = 0;

        for (var i = 0; i < body.Length; i++)
        {
            var current = body[i];
            if (current is (byte)'\r' or (byte)'\n')
                continue;

            if (current == (byte)'=')
            {
                // The escape character and its code may be separated by the line framing.
                var next = i + 1;
                while (next < body.Length && body[next] is (byte)'\r' or (byte)'\n')
                    next++;

                if (next < body.Length && TryDecodeEscape(body[next], out var decoded))
                {
                    destination[written++] = decoded;
                    i = next;
                    continue;
                }
            }

            destination[written++] = current;
        }

        return written;
    }

    private static bool TryDecodeEscape(byte code, out byte value)
    {
        switch (code)
        {
            case (byte)'A':
                value = 0x00;
                return true;
            case (byte)'B':
                value = (byte)'\r';
                return true;
            case (byte)'C':
                value = (byte)'\n';
                return true;
            case (byte)'D':
                value = (byte)'=';
                return true;
            default:
                value = 0;
                return false;
        }
    }
}
