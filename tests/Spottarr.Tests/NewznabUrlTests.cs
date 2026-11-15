using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Spottarr.Configuration;
using Spottarr.Configuration.Options;
using Spottarr.Data.Entities;
using Spottarr.Web.Helpers;
using Spottarr.Web.Newznab;

namespace Spottarr.Tests;

internal sealed class NewznabUrlTests
{
    [Test]
    public async Task NzbUrlDefaultsToGuid()
    {
        var context = CreateRequestContext();
        var options = new NewznabOptions();

        var uri = context.Request.GetNzbUri(6337, options.UseIdForNzbUrls).Uri;

        await Assert
            .That(uri.AbsoluteUri)
            .IsEqualTo("https://example.com:8383/newznab/api?t=get&guid=6337");
    }

    [Test]
    [Arguments(false, "guid")]
    [Arguments(true, "id")]
    public async Task NzbUrlUsesConfiguredIdentifierInBothDownloadLinks(
        bool useIdForNzbUrls,
        string parameter
    )
    {
        var context = CreateRequestContext();
        var uri = context.Request.GetNzbUri(6337, useIdForNzbUrls).Uri;
        var spot = new Spot
        {
            Id = 6337,
            Title = "Test spot",
            SpottedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
            UpdatedAt = new DateTime(2025, 1, 1, 0, 0, 0, DateTimeKind.Utc),
        };
        var item = spot.ToSyndicationItem(context.Request.GetDetailsUri(spot.Id).Uri, uri);

        await Assert.That(uri.Query).IsEqualTo($"?t=get&{parameter}=6337");
        await Assert
            .That(item.Links.Any(link => link.RelationshipType == "alternate" && link.Uri == uri))
            .IsTrue();
        await Assert
            .That(item.Links.Single(link => link.RelationshipType == "enclosure").Uri)
            .IsEqualTo(uri);

        await Assert.That(context.Request.GetNzbUri(6338, useIdForNzbUrls).Uri).IsNotEqualTo(uri);
    }

    [Test]
    public async Task IdUrlSettingBindsFromNewznabConfiguration()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(
                new Dictionary<string, string?> { ["Newznab:UseIdForNzbUrls"] = "true" }
            )
            .Build();
        using var services = new ServiceCollection()
            .AddSpottarrConfiguration(configuration)
            .BuildServiceProvider();

        var options = services.GetRequiredService<IOptions<NewznabOptions>>().Value;

        await Assert.That(options.UseIdForNzbUrls).IsTrue();
    }

    private static DefaultHttpContext CreateRequestContext()
    {
        var context = new DefaultHttpContext();
        context.Request.Scheme = "https";
        context.Request.Host = new HostString("example.com", 8383);
        return context;
    }
}
