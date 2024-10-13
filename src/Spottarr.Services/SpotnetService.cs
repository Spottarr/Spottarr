using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spottarr.Data;
using Spottarr.Services.Configuration;
using Spottarr.Services.Contracts;
using Spottarr.Services.Models;
using Spottarr.Services.Parsers;
using Usenet.Nntp;
using Usenet.Nntp.Models;

namespace Spottarr.Services;

internal sealed class SpotnetService : ISpotnetService
{
    private const int BatchSize = 1000;
    private readonly ILogger<SpotnetService> _logger;
    private readonly IOptions<UsenetOptions> _usenetOptions;
    private readonly IOptions<SpotnetOptions> _spotnetOptions;
    private readonly SpottarrDbContext _dbContext;

    public SpotnetService(ILoggerFactory loggerFactory, ILogger<SpotnetService> logger,
        IOptions<UsenetOptions> usenetOptions, IOptions<SpotnetOptions> spotnetOptions, SpottarrDbContext dbContext)
    {
        _logger = logger;
        _usenetOptions = usenetOptions;
        _spotnetOptions = spotnetOptions;
        _dbContext = dbContext;

        // Enable NNTP client logging
        Usenet.Logger.Factory = loggerFactory;
    }

    public async Task Import()
    {
        // Set up Usenet connection
        using var connection = new NntpConnection();
        var client = new NntpClient(connection);

        var usenetOptions = _usenetOptions.Value;
        await client.ConnectAsync(usenetOptions.Hostname, usenetOptions.Port, usenetOptions.UseTls);
        client.Authenticate(usenetOptions.Username, usenetOptions.Password);

        var spotnetOptions = _spotnetOptions.Value;
        var groupResponse = client.Group(spotnetOptions.SpotGroup);

        if (!groupResponse.Success)
        {
            _logger.CouldNotRetrieveSpotGroup(spotnetOptions.SpotGroup, groupResponse.Code, groupResponse.Message);
            client.Quit();
            return;
        }

        var group = groupResponse.Group;
        var spots = GetSpots(group, client, spotnetOptions.RetrieveAfter, spotnetOptions.RetrieveCount).ToList();

        foreach (var spot in spots)
        {
            Console.WriteLine(spot.Subject);
        }

        client.Quit();
    }

    private IEnumerable<SpotnetHeader> GetSpots(NntpGroup group, NntpClient client, DateTimeOffset retrieveAfter,
        int retrieveCount)
    {
        var from = retrieveCount > 0 ? group.HighWaterMark - retrieveCount : group.LowWaterMark;
        var to = group.HighWaterMark;
        var batches = GetBatches(from, to).ToList();

        foreach (var batch in batches)
        {
            var xOverResponse = client.Xover(batch);
            if (!xOverResponse.Success)
            {
                _logger.CouldNotRetrieveArticles(batch.From, batch.To, xOverResponse.Code, xOverResponse.Message);
                continue;
            }

            foreach (var header in xOverResponse.Lines)
            {
                if (header == null)
                    continue;

                var nntpHeader = NntpHeaderParser.Parse(header);

                if (nntpHeader.Date < retrieveAfter)
                    yield break;

                yield return SpotnetHeaderParser.Parse(nntpHeader);
            }
        }
    }

    private static IEnumerable<NntpArticleRange> GetBatches(long lowWaterMark, long highWaterMark)
    {
        var batchEnd = highWaterMark;
        while (batchEnd >= lowWaterMark)
        {
            var batchStart = Math.Max(lowWaterMark, batchEnd - (BatchSize - 1));
            
            // Make sure that the final batch is inclusive
            if (batchStart - 1 == lowWaterMark) batchStart = lowWaterMark;

            yield return new NntpArticleRange(batchStart, batchEnd);
            
            batchEnd = batchStart - 1;
        }
    }
}