using Spottarr.Services.Parsers;

namespace Spottarr.Tests;

internal sealed class SpotnetXmlParserTests
{
    [Test]
    public async Task ParsesXmlValidVariant1(CancellationToken cancellationToken)
    {
        const string xml = """
            <Spotnet>
                <Posting>
                    <Key>7</Key>
                    <Created>1728935794</Created>
                    <Poster>SomePoster</Poster>
                    <Title>Echoes of Tomorrow - S04E01: A New Dawn</Title>
                    <Description>In a world where people can glimpse fleeting moments of their future, a secretive government agency uses this ability to prevent catastrophic events. As Season 4 begins, former agent Maya Quinn is forced out of hiding to confront a mysterious new threat. With the timelines growing unstable, Maya and her old team must race against time to stop a rogue faction determined to rewrite history. S04E01, "A New Dawn," kicks off the season with shocking revelations and a dangerous new mission that will challenge everything they know about their powers.</Description>
                    <Image Height="1000" Width="680">
                        <Segment>someid1@spot.net</Segment>
                    </Image>
                    <Size>864501308</Size>
                    <Category>01<Sub>01a09</Sub>
                        <Sub>01b04</Sub>
                    </Category>
                    <NZB>
                        <Segment>someid2@spot.net</Segment>
                    </NZB>
                </Posting>
            </Spotnet>
            """;

        var parsed = await SpotnetXmlParser.Parse(xml, cancellationToken);
        await Assert.That(parsed.HasError).IsFalse();
        var result = parsed.Result;

        await Assert.That(result.Posting.Poster).IsEqualTo("SomePoster");
        await Assert
            .That(result.Posting.Title)
            .IsEqualTo("Echoes of Tomorrow - S04E01: A New Dawn");
        await Assert.That(result.Posting.Category.Text).IsEqualTo("01");
        await Assert.That(result.Posting.Category.Sub).IsEquivalentTo(["01a09", "01b04"]);
        await Assert.That(result.Posting.Nzb.Segment).IsEqualTo("someid2@spot.net");
    }

    [Test]
    public async Task ParsesXmlValidVariant2(CancellationToken cancellationToken)
    {
        const string xml = """
            <Spotnet>
            	<Posting>
            		<Key>7</Key>
            		<Created>1729278723</Created>
            		<Poster>SomePoster</Poster>
            		<Title>Midnight Requiem - S01E09: The Final Note</Title>
            		<Description>// OWN RIP //[br]In the dark and haunting city of Vesper, an ancient curse ties the fate of its residents to a series of mysterious, nocturnal deaths.[br][br]Rookie detective Elara Kane, gifted with the ability to hear echoes of the past through music, teams up with a reclusive composer to uncover the truth behind the deaths. In S01E09, "The Final Note," Elara finally closes in on the elusive killer, but a startling discovery about her own past forces her to question everything she thought she knew. Tensions rise as the melody of doom plays its last, chilling refrain.</Description>
            		<Image Width='568' Height='319'>
            			<Segment>someid3@spot.net</Segment>
            		</Image>
            		<Size>3158689370</Size>
            		<Category>01<Sub>01a09</Sub>
            			<Sub>01b04</Sub>
            		</Category>
            		<NZB>
            			<Segment>someid4@spot.net</Segment>
            		</NZB>
            		<PREVSPOTS></PREVSPOTS>
            		<Filename>filename.mp4</Filename>
            		<Newsgroup>some.group</Newsgroup>
            	</Posting>
            </Spotnet>
            """;

        var parsed = await SpotnetXmlParser.Parse(xml, cancellationToken);
        await Assert.That(parsed.HasError).IsFalse();
        var result = parsed.Result;

        await Assert.That(result.Posting.Poster).IsEqualTo("SomePoster");
        await Assert
            .That(result.Posting.Title)
            .IsEqualTo("Midnight Requiem - S01E09: The Final Note");
        await Assert.That(result.Posting.Category.Text).IsEqualTo("01");
        await Assert.That(result.Posting.Filename).IsEqualTo("filename.mp4");
        await Assert.That(result.Posting.Newsgroup).IsEqualTo("some.group");
        await Assert.That(result.Posting.Category.Sub).IsEquivalentTo(["01a09", "01b04"]);
        await Assert.That(result.Posting.Nzb.Segment).IsEqualTo("someid4@spot.net");
    }

    [Test]
    public async Task ParsesXmlValidOutOfOrderWithUnknownFields(CancellationToken cancellationToken)
    {
        const string xml = """
            <?xml version="1.0" encoding="UTF-8"?>
            <Spotnet>
                <Posting SomeUnknownAttribute="SomeValue">
                    <Created>1728935794</Created>
                    <Poster>SomePoster</Poster>
                    <Description>In a world where people can glimpse fleeting moments of their future, a secretive government agency uses this ability to prevent catastrophic events. As Season 4 begins, former agent Maya Quinn is forced out of hiding to confront a mysterious new threat. With the timelines growing unstable, Maya and her old team must race against time to stop a rogue faction determined to rewrite history. S04E01, "A New Dawn," kicks off the season with shocking revelations and a dangerous new mission that will challenge everything they know about their powers.</Description>
                    <Title SomeUnknownAttribute="SomeValue">Echoes of Tomorrow - S04E01: A New Dawn</Title>
                    <SomeUnknownField>SomeValue</SomeUnknownField>
                    <NZB>
                        <Segment>someid2@spot.net</Segment>
                        <SomeUnknownField>SomeValue</SomeUnknownField>
                    </NZB>
                    <Image Width="680" Height="1000" SomeUnknownAttribute="SomeValue">
                        <SomeUnknownField>SomeValue</SomeUnknownField>
                        <Segment>someid1@spot.net</Segment>
                    </Image>
                    <Size>864501308</Size>
                    <Category>01<Sub>01a09</Sub>
                        <Sub>01b04</Sub>
                        <SomeUnknownField>SomeValue</SomeUnknownField>
                    </Category>
                    <Key>7</Key>
                </Posting>
            </Spotnet>
            """;

        var parsed = await SpotnetXmlParser.Parse(xml, cancellationToken);
        await Assert.That(parsed.HasError).IsFalse();
        var result = parsed.Result;

        await Assert.That(result.Posting.Poster).IsEqualTo("SomePoster");
        await Assert
            .That(result.Posting.Title)
            .IsEqualTo("Echoes of Tomorrow - S04E01: A New Dawn");
        await Assert.That(result.Posting.Category.Text).IsEqualTo("01");
        await Assert.That(result.Posting.Category.Sub).IsEquivalentTo(["01a09", "01b04"]);
        await Assert.That(result.Posting.Nzb.Segment).IsEqualTo("someid2@spot.net");
    }

    [Test]
    public async Task ParsesXmlInvalidCharacters(CancellationToken cancellationToken)
    {
        const string xml = """
            <Spotnet>
            	<Posting>
            		<Key>7</Key>
            		<Created>1729278723</Created>
            		<Poster>SomePoster</Poster>
            		<Title>Midnight Requiem - S01E09: The Final Note</Title>
            		<Description>Test &#55357;&#56845;</Description>
            		<Image Width='568' Height='319'>
            			<Segment>someid3@spot.net</Segment>
            		</Image>
            		<Size>3158689370</Size>
            		<Category>01<Sub>01a09</Sub>
            			<Sub>01b04</Sub>
            		</Category>
            		<NZB>
            			<Segment>someid4@spot.net</Segment>
            		</NZB>
            	</Posting>
            </Spotnet>
            """;

        var parsed = await SpotnetXmlParser.Parse(xml, cancellationToken);
        await Assert.That(parsed.HasError).IsFalse();
        var result = parsed.Result;

        await Assert.That(result.Posting.Poster).IsEqualTo("SomePoster");
        await Assert
            .That(result.Posting.Title)
            .IsEqualTo("Midnight Requiem - S01E09: The Final Note");
        await Assert.That(result.Posting.Description).IsEqualTo("Test \ud83d\ude0d");
        await Assert.That(result.Posting.Category.Text).IsEqualTo("01");
        await Assert.That(result.Posting.Category.Sub).IsEquivalentTo(["01a09", "01b04"]);
        await Assert.That(result.Posting.Nzb.Segment).IsEqualTo("someid4@spot.net");
    }

    [Test]
    public async Task ParsesXmlNoWhitespaceBetweenImageAttributes1(
        CancellationToken cancellationToken
    )
    {
        const string xml = """
            <Spotnet>
                <Posting>
                    <Key>7</Key>
                    <Created>1728935794</Created>
                    <Poster>SomePoster</Poster>
                    <Title>Echoes of Tomorrow - S04E01: A New Dawn</Title>
                    <Description>In a world where people can glimpse fleeting moments of their future, a secretive government agency uses this ability to prevent catastrophic events. As Season 4 begins, former agent Maya Quinn is forced out of hiding to confront a mysterious new threat. With the timelines growing unstable, Maya and her old team must race against time to stop a rogue faction determined to rewrite history. S04E01, "A New Dawn," kicks off the season with shocking revelations and a dangerous new mission that will challenge everything they know about their powers.</Description>
                    <Image Width="680"Height="1000">
                        <Segment>someid1@spot.net</Segment>
                    </Image>
                    <Size>864501308</Size>
                    <Category>01<Sub>01a09</Sub>
                        <Sub>01b04</Sub>
                    </Category>
                    <NZB>
                        <Segment>someid2@spot.net</Segment>
                    </NZB>
                </Posting>
            </Spotnet>
            """;

        var parsed = await SpotnetXmlParser.Parse(xml, cancellationToken);
        await Assert.That(parsed.HasError).IsFalse();
        var result = parsed.Result;

        await Assert.That(result.Posting.Poster).IsEqualTo("SomePoster");
        await Assert
            .That(result.Posting.Title)
            .IsEqualTo("Echoes of Tomorrow - S04E01: A New Dawn");
        await Assert.That(result.Posting.Category.Text).IsEqualTo("01");
        await Assert.That(result.Posting.Category.Sub).IsEquivalentTo(["01a09", "01b04"]);
        await Assert.That(result.Posting.Nzb.Segment).IsEqualTo("someid2@spot.net");
    }

    [Test]
    public async Task ParsesXmlNoWhitespaceBetweenImageAttributes2(
        CancellationToken cancellationToken
    )
    {
        const string xml = """
            <Spotnet>
                <Posting>
                    <Key>7</Key>
                    <Created>1728935794</Created>
                    <Poster>SomePoster</Poster>
                    <Title>Echoes of Tomorrow - S04E01: A New Dawn</Title>
                    <Description>In a world where people can glimpse fleeting moments of their future, a secretive government agency uses this ability to prevent catastrophic events. As Season 4 begins, former agent Maya Quinn is forced out of hiding to confront a mysterious new threat. With the timelines growing unstable, Maya and her old team must race against time to stop a rogue faction determined to rewrite history. S04E01, "A New Dawn," kicks off the season with shocking revelations and a dangerous new mission that will challenge everything they know about their powers.</Description>
                    <ImageWidth="680" Height="1000">
                        <Segment>someid1@spot.net</Segment>
                    </Image>
                    <Size>864501308</Size>
                    <Category>01<Sub>01a09</Sub>
                        <Sub>01b04</Sub>
                    </Category>
                    <NZB>
                        <Segment>someid2@spot.net</Segment>
                    </NZB>
                </Posting>
            </Spotnet>
            """;

        var parsed = await SpotnetXmlParser.Parse(xml, cancellationToken);
        await Assert.That(parsed.HasError).IsFalse();
        var result = parsed.Result;

        await Assert.That(result.Posting.Poster).IsEqualTo("SomePoster");
        await Assert
            .That(result.Posting.Title)
            .IsEqualTo("Echoes of Tomorrow - S04E01: A New Dawn");
        await Assert.That(result.Posting.Category.Text).IsEqualTo("01");
        await Assert.That(result.Posting.Category.Sub).IsEquivalentTo(["01a09", "01b04"]);
        await Assert.That(result.Posting.Nzb.Segment).IsEqualTo("someid2@spot.net");
    }

    [Test]
    public async Task ParsesXmlValidLines(CancellationToken cancellationToken)
    {
        const string xml = """
            <Spotnet>
                <Posting>
                    <Key>7</Key>
                    <Created>1728935794</Created>
                    <Poster>SomePoster</Poster>
                    <Title>Echoes of Tomorrow - S04E01: A New Dawn</Title>
                    <Description>In a world where people can glimpse fleeting moments of their future, a secretive government agency uses this ability to prevent catastrophic events. As Season 4 begins, former agent Maya Quinn is forced out of hiding to confront a mysterious new threat. With the timelines growing unstable, Maya and her old team must race against time to stop a rogue faction determined to rewrite history. S04E01, "A New Dawn," kicks off the season with shocking revelations and a dangerous new mission that will challenge everything they know about their powers.</Description>
                    <Image Height="1000" Width="680">
                        <Segment>someid1@spot.net</Segment>
                    </Image>
                    <Size>864501308</Size>
                    <Category>01<Sub>01a09</Sub>
                        <Sub>01b04</Sub>
                    </Category>
                    <NZB>
                        <Segment>someid2@spot.net</Segment>
                    </NZB>
                </Posting>
            </Spotnet>
            """;

        var lines = xml.Split("\n", StringSplitOptions.RemoveEmptyEntries).ToList();
        var parsed = await SpotnetXmlParser.Parse(lines, cancellationToken);
        await Assert.That(parsed.HasError).IsFalse();
        var result = parsed.Result;

        await Assert.That(result.Posting.Poster).IsEqualTo("SomePoster");
        await Assert
            .That(result.Posting.Title)
            .IsEqualTo("Echoes of Tomorrow - S04E01: A New Dawn");
        await Assert.That(result.Posting.Category.Text).IsEqualTo("01");
        await Assert.That(result.Posting.Category.Sub).IsEquivalentTo(["01a09", "01b04"]);
        await Assert.That(result.Posting.Nzb.Segment).IsEqualTo("someid2@spot.net");
    }
}
