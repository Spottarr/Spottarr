using Spottarr.Services.Parsers;
using Xunit;

namespace Spottarr.Tests;

public class SpotnetXmlParserTests
{
    [Fact]
    public void ParsesXmlValidVariant1()
    {
        const string xml = """
                           <Spotnet>
                               <Posting>
                                   <Key>7</Key>
                                   <Created>1728935794</Created>
                                   <Poster>SomePoster</Poster>
                                   <Title>Echoes of Tomorrow - S04E01: A New Dawn</Title>
                                   <Description>In a world where people can glimpse fleeting moments of their future, a secretive government agency uses this ability to prevent catastrophic events. As Season 4 begins, former agent Maya Quinn is forced out of hiding to confront a mysterious new threat. With the timelines growing unstable, Maya and her old team must race against time to stop a rogue faction determined to rewrite history. S04E01, “A New Dawn,” kicks off the season with shocking revelations and a dangerous new mission that will challenge everything they know about their powers.</Description>
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
        var result = SpotnetXmlParser.Parse(xml);

        Assert.Equal("SomePoster", result.Posting.Poster);
        Assert.Equal("Echoes of Tomorrow - S04E01: A New Dawn", result.Posting.Title);
        Assert.Equal("01", result.Posting.Category.Text);
        Assert.Collection(result.Posting.Category.Sub, sub1 => { Assert.Equal("01a09", sub1); },
            sub2 => { Assert.Equal("01b04", sub2); });
        Assert.Equal("someid2@spot.net", result.Posting.Nzb.Segment);
    }

    [Fact]
    public void ParsesXmlValidVariant2()
    {
        const string xml = """
                           <Spotnet>
                           	<Posting>
                           		<Key>7</Key>
                           		<Created>1729278723</Created>
                           		<Poster>SomePoster</Poster>
                           		<Title>Midnight Requiem - S01E09: The Final Note</Title>
                           		<Description>// OWN RIP //[br]In the dark and haunting city of Vesper, an ancient curse ties the fate of its residents to a series of mysterious, nocturnal deaths.[br][br]Rookie detective Elara Kane, gifted with the ability to hear echoes of the past through music, teams up with a reclusive composer to uncover the truth behind the deaths. In S01E09, “The Final Note,” Elara finally closes in on the elusive killer, but a startling discovery about her own past forces her to question everything she thought she knew. Tensions rise as the melody of doom plays its last, chilling refrain.</Description>
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
                           	</Posting>
                           </Spotnet>
                           """;

        var result = SpotnetXmlParser.Parse(xml);

        Assert.Equal("SomePoster", result.Posting.Poster);
        Assert.Equal("Midnight Requiem - S01E09: The Final Note", result.Posting.Title);
        Assert.Equal("01", result.Posting.Category.Text);
        Assert.Collection(result.Posting.Category.Sub, sub1 => { Assert.Equal("01a09", sub1); },
            sub2 => { Assert.Equal("01b04", sub2); });
        Assert.Equal("someid4@spot.net", result.Posting.Nzb.Segment);
    }

    [Fact]
    public void ParsesXmlInvalidCharacters()
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
                           		<PREVSPOTS></PREVSPOTS>
                           	</Posting>
                           </Spotnet>
                           """;

        var result = SpotnetXmlParser.Parse(xml);

        Assert.Equal("SomePoster", result.Posting.Poster);
        Assert.Equal("Midnight Requiem - S01E09: The Final Note", result.Posting.Title);
        Assert.Equal("Test \ud83d\ude0d", result.Posting.Description);
        Assert.Equal("01", result.Posting.Category.Text);
        Assert.Collection(result.Posting.Category.Sub, sub1 => { Assert.Equal("01a09", sub1); },
            sub2 => { Assert.Equal("01b04", sub2); });
        Assert.Equal("someid4@spot.net", result.Posting.Nzb.Segment);
    }

    [Fact]
    public void DoesNotParseXmlInvalidImageTag()
    {
        const string xml = """
                           <Spotnet>
                               <Posting>
                                   <Key>7</Key>
                                   <Created>1729126073</Created>
                                   <Poster>SomePoster</Poster>
                                   <Title>Echoes of Tomorrow - S04E01: A New Dawn</Title>
                                   <Description>In a world where people can glimpse fleeting moments of their future, a secretive government agency uses this ability to prevent catastrophic events. As Season 4 begins, former agent Maya Quinn is forced out of hiding to confront a mysterious new threat. With the timelines growing unstable, Maya and her old team must race against time to stop a rogue faction determined to rewrite history. S04E01, “A New Dawn,” kicks off the season with shocking revelations and a dangerous new mission that will challenge everything they know about their powers.</Description>
                                   <ImageWidth='900' Height='600'>
                                       <Segment>someid1@spot.net</Segment>
                                   </Image>
                                   <Size>447177625</Size>
                                   <Category>01<Sub>01a09</Sub>
                                       <Sub>01b04</Sub>
                                   </Category>
                                   <NZB>
                                       <Segment>someid4@spot.net</Segment>
                                   </NZB>
                               </Posting>
                           </Spotnet>
                           """;

        Assert.Throws<InvalidOperationException>(() => SpotnetXmlParser.Parse(xml));
    }
}