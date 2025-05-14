using System.ServiceModel.Syndication;
using Spottarr.Data.Entities;
using Spottarr.Data.Entities.Enums;
using Spottarr.Web.Newznab;
using Xunit;

namespace Spottarr.Tests;

public class NewznabRssSerializerTests
{
    [Fact]
    public async Task XmlValid()
    {
        List<Spot> spots =
        [
            new()
            {
                Title = "Testspot大",
                Spotter = "Testspotter",
                Bytes = 1000,
                MessageId = "1234",
                NzbMessageId = null,
                ImageMessageId = null,
                MessageNumber = 1234,
                Type = SpotType.None,
                SpottedAt = new DateTime(2025, 1, 1, 12, 0, 0),
                CreatedAt = new DateTime(2025, 1, 1, 12, 0, 0),
                UpdatedAt = new DateTime(2025, 1, 1, 12, 0, 0),
            }
        ];

        var items = spots.Select(s => s.ToSyndicationItem(new Uri("https://example.com/nzb"))).ToList();

        var feed = new SyndicationFeed("Test", "Test", new Uri("https://example.com"), items)
            .AddLogo(new Uri("https://example.com/logo.png"))
            .AddNewznabNamespace()
            .AddNewznabResponseInfo(0, items.Count);

        using var ms = NewznabRssSerializer.Serialize(feed);
        using var sr = new StreamReader(ms);

        var expected = """
                       <?xml version="1.0" encoding="utf-8"?>
                       <rss version="2.0">
                         <channel xmlns:newznab="https://www.newznab.com/DTD/2010/feeds/attributes/">
                           <title>Test</title>
                           <link>https://example.com/</link>
                           <description>Test</description>
                           <image>
                             <url>https://example.com/logo.png</url>
                             <title>Test</title>
                             <link>https://example.com/</link>
                           </image>
                           <newznab:response offset="0" total="1" />
                           <item>
                             <guid isPermaLink="false">0</guid>
                             <link>https://example.com/nzb</link>
                             <title>Testspot大</title>
                             <pubDate>Wed, 01 Jan 2025 12:00:00 +0100</pubDate>
                             <enclosure url="https://example.com/nzb" type="application/x-nzb" length="1000" />
                             <newznab:attr name="size" value="1000" />
                             <newznab:attr name="guid" value="0" />
                             <newznab:attr name="poster" value="Testspotter" />
                             <newznab:attr name="team" value="Testspotter" />
                             <newznab:attr name="usenetdate" value="Wed, 01 Jan 2025 12:00:00 GMT" />
                             <newznab:attr name="year" value="" />
                           </item>
                         </channel>
                       </rss>
                       """;

        var actual = await sr.ReadToEndAsync(TestContext.Current.CancellationToken);

        Assert.Equal(expected, actual);
    }

    [Fact]
    public async Task XmlInvalidChar()
    {
        List<Spot> spots =
        [
            new()
            {
                Title = "Testspot\x1F",
                Spotter = "Testspotter",
                Bytes = 1000,
                MessageId = "1234",
                NzbMessageId = null,
                ImageMessageId = null,
                MessageNumber = 1234,
                Type = SpotType.None,
                SpottedAt = new DateTime(2025, 1, 1, 12, 0, 0),
                CreatedAt = new DateTime(2025, 1, 1, 12, 0, 0),
                UpdatedAt = new DateTime(2025, 1, 1, 12, 0, 0),
            }
        ];

        var items = spots.Select(s => s.ToSyndicationItem(new Uri("https://example.com/nzb"))).ToList();

        var feed = new SyndicationFeed("Test", "Test", new Uri("https://example.com"), items)
            .AddLogo(new Uri("https://example.com/logo.png"))
            .AddNewznabNamespace()
            .AddNewznabResponseInfo(0, items.Count);

        using var ms = NewznabRssSerializer.Serialize(feed);
        using var sr = new StreamReader(ms);

        var expected = """
                       <?xml version="1.0" encoding="utf-8"?>
                       <rss version="2.0">
                         <channel xmlns:newznab="https://www.newznab.com/DTD/2010/feeds/attributes/">
                           <title>Test</title>
                           <link>https://example.com/</link>
                           <description>Test</description>
                           <image>
                             <url>https://example.com/logo.png</url>
                             <title>Test</title>
                             <link>https://example.com/</link>
                           </image>
                           <newznab:response offset="0" total="1" />
                           <item>
                             <guid isPermaLink="false">0</guid>
                             <link>https://example.com/nzb</link>
                             <title>Testspot?</title>
                             <pubDate>Wed, 01 Jan 2025 12:00:00 +0100</pubDate>
                             <enclosure url="https://example.com/nzb" type="application/x-nzb" length="1000" />
                             <newznab:attr name="size" value="1000" />
                             <newznab:attr name="guid" value="0" />
                             <newznab:attr name="poster" value="Testspotter" />
                             <newznab:attr name="team" value="Testspotter" />
                             <newznab:attr name="usenetdate" value="Wed, 01 Jan 2025 12:00:00 GMT" />
                             <newznab:attr name="year" value="" />
                           </item>
                         </channel>
                       </rss>
                       """;

        var actual = await sr.ReadToEndAsync(TestContext.Current.CancellationToken);

        Assert.Equal(expected, actual);
    }
}