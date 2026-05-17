using Spottarr.Services.Parsers;

namespace Spottarr.Tests;

internal sealed class NntpHeaderParserTests
{
    [Test]
    public async Task ParsesValidHeader()
    {
        const string header =
            "123\tTest Subject\tTest Author\tWed, 01 Nov 2023 12:34:56 +0000\t<msgid@host>\t<ref>\t456\t7";

        var result = NntpHeaderParser.Parse(header);
        await Assert.That(result.HasError).IsFalse();
        await Assert.That(result.Result.ArticleNumber).IsEqualTo(123);
        await Assert.That(result.Result.Subject).IsEqualTo("Test Subject");
        await Assert.That(result.Result.Author).IsEqualTo("Test Author");
        await Assert.That(result.Result.MessageId).IsEqualTo("msgid@host");
        await Assert.That(result.Result.References).IsEqualTo("<ref>");
        await Assert.That(result.Result.Bytes).IsEqualTo(456);
        await Assert.That(result.Result.Lines).IsEqualTo(7);
    }

    [Test]
    public async Task ReturnsErrorOnTooFewFields()
    {
        const string header = "123\tTest Subject\tTest Author";

        var result = NntpHeaderParser.Parse(header);
        await Assert.That(result.HasError).IsTrue();
        await Assert.That(result.Error).IsEqualTo("Expected 8 header fields, got 3");
    }
}
