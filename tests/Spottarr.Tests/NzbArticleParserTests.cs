using System.IO.Compression;
using System.Text;
using Spottarr.Services.Parsers;

namespace Spottarr.Tests;

internal sealed class NzbArticleParserTests
{
    private const string NzbXml = """
        <?xml version="1.0" encoding="iso-8859-1"?>
        <nzb xmlns="http://www.newzbin.com/DTD/2003/nzb">
          <file poster="SomePoster &lt;poster@spot.net&gt;" date="1728935794" subject="[01/12] - &quot;Echoes.of.Tomorrow.S04.par2&quot; yEnc (1/1)">
            <groups>
              <group>alt.binaries.test</group>
            </groups>
            <segments>
              <segment bytes="384000" number="1">part1of12@spot.net</segment>
            </segments>
          </file>
          <file poster="SomePoster &lt;poster@spot.net&gt;" date="1728935795" subject="[02/12] - &quot;Echoes.of.Tomorrow.S04.r00&quot; yEnc (1/3)">
            <groups>
              <group>alt.binaries.test</group>
            </groups>
            <segments>
              <segment bytes="768000" number="1">part2of12a@spot.net</segment>
              <segment bytes="768000" number="2">part2of12b@spot.net</segment>
              <segment bytes="512000" number="3">part2of12c@spot.net</segment>
            </segments>
          </file>
        </nzb>
        """;

    [Test]
    public async Task DecodesAttachmentSplitOverMultipleSegments(
        CancellationToken cancellationToken
    )
    {
        var bodies = BuildSegmentBodies(NzbXml, segmentCount: 3);

        await Assert.That(bodies.Count).IsEqualTo(3);

        var result = await NzbArticleParser.Parse(Concat(bodies), cancellationToken);

        await Assert.That(ReadAsString(result)).IsEqualTo(NzbXml);
    }

    // zlib rejects a stream missing its head with Z_DATA_ERROR, which .NET surfaces as
    // "unsupported compression method".
    [Test]
    public async Task ThrowsWhenOnlyTheLastSegmentIsDecoded(CancellationToken cancellationToken)
    {
        var bodies = BuildSegmentBodies(NzbXml, segmentCount: 3);

        await Assert
            .That(async () => await NzbArticleParser.Parse(bodies[^1], cancellationToken))
            .Throws<InvalidDataException>();
    }

    [Test]
    public async Task DecodesAttachmentInASingleSegment(CancellationToken cancellationToken)
    {
        var bodies = BuildSegmentBodies(NzbXml, segmentCount: 1);

        var result = await NzbArticleParser.Parse(bodies[0], cancellationToken);

        await Assert.That(ReadAsString(result)).IsEqualTo(NzbXml);
    }

    /// <summary>
    /// Deflates the XML, escapes it with the =A/=B/=C/=D scheme, splits it over
    /// <paramref name="segmentCount"/> article bodies and adds CRLF line framing.
    /// Splits land between an escape character and its code, the case that breaks when segments are
    /// unescaped individually instead of concatenated first.
    /// </summary>
    private static List<ReadOnlyMemory<byte>> BuildSegmentBodies(string xml, int segmentCount)
    {
        var escaped = Escape(Deflate(xml));
        var splitPoints = FindEscapePairSplitPoints(escaped, segmentCount - 1);

        var bodies = new List<ReadOnlyMemory<byte>>(segmentCount);
        var start = 0;
        foreach (var end in splitPoints.Append(escaped.Length))
        {
            bodies.Add(AddLineFraming(escaped.AsSpan(start..end)));
            start = end;
        }

        return bodies;
    }

    private static byte[] Deflate(string xml)
    {
        using var output = new MemoryStream();
        using (var deflate = new DeflateStream(output, CompressionLevel.Optimal, leaveOpen: true))
        {
            var bytes = Encoding.UTF8.GetBytes(xml);
            deflate.Write(bytes, 0, bytes.Length);
        }

        return output.ToArray();
    }

    private static byte[] Escape(ReadOnlySpan<byte> compressed)
    {
        var output = new List<byte>(compressed.Length);
        foreach (var b in compressed)
        {
            switch (b)
            {
                case 0x00:
                    output.AddRange("=A"u8);
                    break;
                case (byte)'\r':
                    output.AddRange("=B"u8);
                    break;
                case (byte)'\n':
                    output.AddRange("=C"u8);
                    break;
                case (byte)'=':
                    output.AddRange("=D"u8);
                    break;
                default:
                    output.Add(b);
                    break;
            }
        }

        return [.. output];
    }

    private static List<int> FindEscapePairSplitPoints(byte[] escaped, int count)
    {
        var points = new List<int>(count);
        if (count == 0)
            return points;

        var stride = escaped.Length / (count + 1);
        for (var i = 1; i <= count; i++)
        {
            var offset = i * stride;
            while (offset < escaped.Length && escaped[offset - 1] != (byte)'=')
                offset++;

            if (offset < escaped.Length)
                points.Add(offset);
        }

        return points;
    }

    // The server frames the body into CRLF-terminated lines; those CRLFs are not part of the payload.
    private static ReadOnlyMemory<byte> AddLineFraming(ReadOnlySpan<byte> body)
    {
        const int lineLength = 128;
        var output = new List<byte>(body.Length);
        for (var i = 0; i < body.Length; i += lineLength)
        {
            var length = Math.Min(lineLength, body.Length - i);
            output.AddRange(body.Slice(i, length));
            output.AddRange("\r\n"u8);
        }

        return output.ToArray();
    }

    private static ReadOnlyMemory<byte> Concat(List<ReadOnlyMemory<byte>> bodies)
    {
        var output = new byte[bodies.Sum(b => b.Length)];
        var offset = 0;
        foreach (var body in bodies)
        {
            body.Span.CopyTo(output.AsSpan(offset));
            offset += body.Length;
        }

        return output;
    }

    private static string ReadAsString(MemoryStream stream)
    {
        using var reader = new StreamReader(stream, Encoding.UTF8);
        return reader.ReadToEnd();
    }
}
