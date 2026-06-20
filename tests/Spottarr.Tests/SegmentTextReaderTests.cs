using Spottarr.Services.Helpers;

namespace Spottarr.Tests;

internal sealed class SegmentTextReaderTests
{
    [Test]
    public async Task ReadsAllSegmentsAsSingleStream()
    {
        using var reader = new SegmentTextReader(["foo", "bar", "baz"]);

        var result = await reader.ReadToEndAsync();

        await Assert.That(result).IsEqualTo("foobarbaz");
    }

    [Test]
    public async Task SkipsEmptySegments()
    {
        using var reader = new SegmentTextReader(["foo", string.Empty, "bar", string.Empty]);

        var result = await reader.ReadToEndAsync();

        await Assert.That(result).IsEqualTo("foobar");
    }

    // Reading with a buffer smaller than a segment forces multiple Read calls within the same segment.
    // The original StringEnumerableReader advanced the enumerator on every Read and silently dropped the
    // remainder of partially consumed segments; this guards against that regression.
    [Test]
    public async Task DoesNotDropSegmentsWhenBufferIsSmallerThanSegment()
    {
        using var reader = new SegmentTextReader(["abcdef", "ghijkl"]);

        var buffer = new char[4];
        var output = new System.Text.StringBuilder();

        int read;
        while ((read = await reader.ReadAsync(buffer, 0, buffer.Length)) > 0)
            output.Append(buffer, 0, read);

        await Assert.That(output.ToString()).IsEqualTo("abcdefghijkl");
    }

    [Test]
    public async Task ReadAndPeekReturnCharactersAcrossSegmentBoundaries()
    {
        using var reader = new SegmentTextReader(["ab", "c"]);

        await Assert.That(reader.Peek()).IsEqualTo('a');
        await Assert.That(reader.Read()).IsEqualTo('a');
        await Assert.That(reader.Read()).IsEqualTo('b');
        await Assert.That(reader.Peek()).IsEqualTo('c');
        await Assert.That(reader.Read()).IsEqualTo('c');
        await Assert.That(reader.Read()).IsEqualTo(-1);
    }
}
