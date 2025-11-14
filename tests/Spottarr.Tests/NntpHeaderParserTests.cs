using Spottarr.Services.Parsers;
using Xunit;

namespace Spottarr.Tests;

public class NntpHeaderParserTests
{
    [Fact]
    public void ParsesValidHeader()
    {
        const string header =
            "123\tTest Subject\tTest Author\tWed, 01 Nov 2023 12:34:56 +0000\t<msgid@host>\t<ref>\t456\t7";

        var result = NntpHeaderParser.Parse(header);
        Assert.False(result.HasError);
        Assert.Equal(123, result.Result.ArticleNumber);
        Assert.Equal("Test Subject", result.Result.Subject);
        Assert.Equal("Test Author", result.Result.Author);
        Assert.Equal("msgid@host", result.Result.MessageId);
        Assert.Equal("<ref>", result.Result.References);
        Assert.Equal(456, result.Result.Bytes);
        Assert.Equal(7, result.Result.Lines);
    }

    [Fact]
    public void ReturnsErrorOnTooFewFields()
    {
        const string header = "123\tTest Subject\tTest Author";

        var result = NntpHeaderParser.Parse(header);
        Assert.True(result.HasError);
        Assert.Equal("Expected 8 header fields, got 3", result.Error);
    }
}