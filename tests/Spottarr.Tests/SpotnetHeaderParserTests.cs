using Spottarr.Services.Nntp;
using Spottarr.Services.Parsers;
using Spottarr.Services.Spotnet;

namespace Spottarr.Tests;

internal sealed class SpotnetHeaderParserTests
{
    [Test]
    [Arguments("")]
    [Arguments("Test subject")]
    public async Task ParsesHeaderValidVariant1(string subjectAndTags)
    {
        const string spotnetHeader =
            "solem <pizlw58KFSC94SUdwAIMBzRNxmCZrhEnZb4ihCLAX9p6ViN9s2vf-pwFFZqPKwzFF.dpeyw-ssEYgAUebInWNwvjKu6irDwuJCTpgDL7Y1k6lBQj1j4YE-sl99LqQ-sjg7fUf@17a09b04d11b11c00c11z01.3366259428.20.1727723059.1.NL.OmepU20o1i7VxNRhQkAxC1MU8UH4fuOy-pmcHCSgOyCv71Qi-pKuuyPFADcZSY2JqI>";
        var nntpHeader = PrepareNntpHeader(spotnetHeader, subjectAndTags);

        var parsingResult = SpotnetHeaderParser.Parse(nntpHeader);

        await Assert.That(parsingResult.HasError).IsFalse();

        var result = parsingResult.Result;

        await Assert.That(result.Nickname).IsEqualTo("solem");
        await Assert
            .That(result.UserModulus)
            .IsEqualTo("pizlw58KFSC94SUdwAIMBzRNxmCZrhEnZb4ihCLAX9p6ViN9s2vf-pwFFZqPKwzFF");
        await Assert
            .That(result.UserSignature)
            .IsEqualTo("dpeyw-ssEYgAUebInWNwvjKu6irDwuJCTpgDL7Y1k6lBQj1j4YE-sl99LqQ-sjg7fUf");
        await Assert.That(result.Category).IsEqualTo(1);
        await Assert.That(result.KeyId).IsEqualTo(KeyId.SelfSigned);

        var subCategories = result.SubCategories.ToList();
        await Assert.That(subCategories).Count().IsEqualTo(7);

        await Assert.That(subCategories[0].Type).IsEqualTo('A');
        await Assert.That(subCategories[0].Code).IsEqualTo(9);

        await Assert.That(subCategories[1].Type).IsEqualTo('B');
        await Assert.That(subCategories[1].Code).IsEqualTo(4);

        await Assert.That(subCategories[2].Type).IsEqualTo('D');
        await Assert.That(subCategories[2].Code).IsEqualTo(11);

        await Assert.That(subCategories[3].Type).IsEqualTo('B');
        await Assert.That(subCategories[3].Code).IsEqualTo(11);

        await Assert.That(subCategories[4].Type).IsEqualTo('C');
        await Assert.That(subCategories[4].Code).IsEqualTo(0);

        await Assert.That(subCategories[5].Type).IsEqualTo('C');
        await Assert.That(subCategories[5].Code).IsEqualTo(11);

        await Assert.That(subCategories[6].Type).IsEqualTo('Z');
        await Assert.That(subCategories[6].Code).IsEqualTo(1);

        await Assert.That(result.Size).IsEqualTo(3366259428);
        await Assert
            .That(result.Date)
            .IsEqualTo(new DateTimeOffset(2024, 09, 30, 19, 04, 19, TimeSpan.Zero));
        await Assert.That(result.CustomId).IsEqualTo("1");
        await Assert.That(result.CustomValue).IsEqualTo("NL");
        await Assert
            .That(result.ServerSignature)
            .IsEqualTo("OmepU20o1i7VxNRhQkAxC1MU8UH4fuOy-pmcHCSgOyCv71Qi-pKuuyPFADcZSY2JqI");
    }

    [Test]
    public async Task ParsesHeaderValidVariant2()
    {
        const string spotnetHeader =
            "Krid <7oGXyqZiTFIad-sLhInYOEcYylEmweUvnYF8CapS5S5hH12pdjwdLpd05fweJAHP5.U8c7LC-sA-sq2-sw0yjuY-s-sDj1bncqGmFVlaQy3UtQyw65WkWdMEf8i3g7ImuVTS1-pc@12a01.999.10.1727721178.1.NL.rV-sGFAvYDLhOVSllN173n2ZbEbq0rkSm9ku9j5CDmSQwKVj0kP-ph8CzB4L2PcEfr>";
        var nntpHeader = PrepareNntpHeader(spotnetHeader);

        var parsingResult = SpotnetHeaderParser.Parse(nntpHeader);

        await Assert.That(parsingResult.HasError).IsFalse();

        var result = parsingResult.Result;

        await Assert.That(result.Nickname).IsEqualTo("Krid");
        await Assert
            .That(result.UserModulus)
            .IsEqualTo("7oGXyqZiTFIad-sLhInYOEcYylEmweUvnYF8CapS5S5hH12pdjwdLpd05fweJAHP5");
        await Assert
            .That(result.UserSignature)
            .IsEqualTo("U8c7LC-sA-sq2-sw0yjuY-s-sDj1bncqGmFVlaQy3UtQyw65WkWdMEf8i3g7ImuVTS1-pc");
        await Assert.That(result.Category).IsEqualTo(1);
        await Assert.That(result.KeyId).IsEqualTo(KeyId.Moderator);

        var subCategories = result.SubCategories.ToList();
        await Assert.That(subCategories).Count().IsEqualTo(1);
        await Assert.That(subCategories[0].Type).IsEqualTo('A');
        await Assert.That(subCategories[0].Code).IsEqualTo(1);

        await Assert.That(result.Size).IsEqualTo(999);
        await Assert
            .That(result.Date)
            .IsEqualTo(new DateTimeOffset(2024, 09, 30, 18, 32, 58, TimeSpan.Zero));
        await Assert.That(result.CustomId).IsEqualTo("1");
        await Assert.That(result.CustomValue).IsEqualTo("NL");
        await Assert
            .That(result.ServerSignature)
            .IsEqualTo("rV-sGFAvYDLhOVSllN173n2ZbEbq0rkSm9ku9j5CDmSQwKVj0kP-ph8CzB4L2PcEfr");
    }

    [Test]
    public async Task DoesNotParseHeaderMissingFields()
    {
        const string spotnetHeader =
            "BeAware <ys2GY-pPgAmyOVPpnUAG5wVj0AK6o0-saMWIsfCmLX4FCg834PdODIE9OhNSwQQ3XR.qoJvSsrXXhL65SUp8zhvdH-pzqhuzVFUpP1FgDqQhIsGJDN01j7yGNsJeOuLslCTL@spot.net>";
        var nntpHeader = PrepareNntpHeader(spotnetHeader);

        var parsingResult = SpotnetHeaderParser.Parse(nntpHeader);

        await Assert.That(parsingResult.HasError).IsTrue();
    }

    private static NntpHeader PrepareNntpHeader(
        string spotnetHeader,
        string subjectAndTags = "Test subject"
    ) =>
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
