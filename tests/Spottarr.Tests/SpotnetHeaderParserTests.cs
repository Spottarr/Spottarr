using Spottarr.Services.Nntp;
using Spottarr.Services.Parsers;
using Spottarr.Services.Spotnet;
using Xunit;

namespace Spottarr.Tests;

public class SpotnetHeaderParserTests
{
    [Theory]
    [InlineData("")]
    [InlineData("Test subject")]
    public void ParsesHeaderValidVariant1(string subjectAndTags)
    {
        const string spotnetHeader =
            "solem <pizlw58KFSC94SUdwAIMBzRNxmCZrhEnZb4ihCLAX9p6ViN9s2vf-pwFFZqPKwzFF.dpeyw-ssEYgAUebInWNwvjKu6irDwuJCTpgDL7Y1k6lBQj1j4YE-sl99LqQ-sjg7fUf@17a09b04d11b11c00c11z01.3366259428.20.1727723059.1.NL.OmepU20o1i7VxNRhQkAxC1MU8UH4fuOy-pmcHCSgOyCv71Qi-pKuuyPFADcZSY2JqI>";
        var nntpHeader = PrepareNntpHeader(spotnetHeader, subjectAndTags);

        var parsingResult = SpotnetHeaderParser.Parse(nntpHeader);

        Assert.False(parsingResult.HasError);

        var result = parsingResult.Result;

        Assert.Equal("solem", result.Nickname);
        Assert.Equal("pizlw58KFSC94SUdwAIMBzRNxmCZrhEnZb4ihCLAX9p6ViN9s2vf-pwFFZqPKwzFF", result.UserModulus);
        Assert.Equal("dpeyw-ssEYgAUebInWNwvjKu6irDwuJCTpgDL7Y1k6lBQj1j4YE-sl99LqQ-sjg7fUf", result.UserSignature);
        Assert.Equal(1, result.Category);
        Assert.Equal(KeyId.SelfSigned, result.KeyId);
        Assert.Collection(result.SubCategories, cat1 =>
        {
            Assert.Equal('A', cat1.Type);
            Assert.Equal(9, cat1.Code);
        }, cat2 =>
        {
            Assert.Equal('B', cat2.Type);
            Assert.Equal(4, cat2.Code);
        }, cat3 =>
        {
            Assert.Equal('D', cat3.Type);
            Assert.Equal(11, cat3.Code);
        }, cat4 =>
        {
            Assert.Equal('B', cat4.Type);
            Assert.Equal(11, cat4.Code);
        }, cat5 =>
        {
            Assert.Equal('C', cat5.Type);
            Assert.Equal(0, cat5.Code);
        }, cat6 =>
        {
            Assert.Equal('C', cat6.Type);
            Assert.Equal(11, cat6.Code);
        }, cat7 =>
        {
            Assert.Equal('Z', cat7.Type);
            Assert.Equal(1, cat7.Code);
        });
        Assert.Equal(3366259428, result.Size);
        Assert.Equal(new DateTimeOffset(2024, 09, 30, 19, 04, 19, TimeSpan.Zero), result.Date);
        Assert.Equal("1", result.CustomId);
        Assert.Equal("NL", result.CustomValue);
        Assert.Equal("OmepU20o1i7VxNRhQkAxC1MU8UH4fuOy-pmcHCSgOyCv71Qi-pKuuyPFADcZSY2JqI", result.ServerSignature);
    }

    [Fact]
    public void ParsesHeaderValidVariant2()
    {
        const string spotnetHeader =
            "Krid <7oGXyqZiTFIad-sLhInYOEcYylEmweUvnYF8CapS5S5hH12pdjwdLpd05fweJAHP5.U8c7LC-sA-sq2-sw0yjuY-s-sDj1bncqGmFVlaQy3UtQyw65WkWdMEf8i3g7ImuVTS1-pc@12a01.999.10.1727721178.1.NL.rV-sGFAvYDLhOVSllN173n2ZbEbq0rkSm9ku9j5CDmSQwKVj0kP-ph8CzB4L2PcEfr>";
        var nntpHeader = PrepareNntpHeader(spotnetHeader);

        var parsingResult = SpotnetHeaderParser.Parse(nntpHeader);

        Assert.False(parsingResult.HasError);

        var result = parsingResult.Result;

        Assert.Equal("Krid", result.Nickname);
        Assert.Equal("7oGXyqZiTFIad-sLhInYOEcYylEmweUvnYF8CapS5S5hH12pdjwdLpd05fweJAHP5", result.UserModulus);
        Assert.Equal("U8c7LC-sA-sq2-sw0yjuY-s-sDj1bncqGmFVlaQy3UtQyw65WkWdMEf8i3g7ImuVTS1-pc", result.UserSignature);
        Assert.Equal(1, result.Category);
        Assert.Equal(KeyId.Moderator, result.KeyId);
        Assert.Collection(result.SubCategories, cat1 =>
        {
            Assert.Equal('A', cat1.Type);
            Assert.Equal(1, cat1.Code);
        });
        Assert.Equal(999, result.Size);
        Assert.Equal(new DateTimeOffset(2024, 09, 30, 18, 32, 58, TimeSpan.Zero), result.Date);
        Assert.Equal("1", result.CustomId);
        Assert.Equal("NL", result.CustomValue);
        Assert.Equal("rV-sGFAvYDLhOVSllN173n2ZbEbq0rkSm9ku9j5CDmSQwKVj0kP-ph8CzB4L2PcEfr", result.ServerSignature);
    }

    [Fact]
    public void DoesNotParseHeaderMissingFields()
    {
        const string spotnetHeader =
            "BeAware <ys2GY-pPgAmyOVPpnUAG5wVj0AK6o0-saMWIsfCmLX4FCg834PdODIE9OhNSwQQ3XR.qoJvSsrXXhL65SUp8zhvdH-pzqhuzVFUpP1FgDqQhIsGJDN01j7yGNsJeOuLslCTL@spot.net>";
        var nntpHeader = PrepareNntpHeader(spotnetHeader);

        var parsingResult = SpotnetHeaderParser.Parse(nntpHeader);

        Assert.True(parsingResult.HasError);
    }

    private static NntpHeader PrepareNntpHeader(string spotnetHeader, string subjectAndTags = "Test subject") =>
        new()
        {
            ArticleNumber = 1,
            Subject = subjectAndTags,
            Author = spotnetHeader,
            Date = default,
            MessageId = "message123@spot.net",
            References = string.Empty,
            Bytes = 0,
            Lines = 0,
        };
}