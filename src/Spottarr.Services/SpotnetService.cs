using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spottarr.Services.Configuration;
using Spottarr.Services.Contracts;
using Spottarr.Services.Models;
using Usenet.Nntp;
using Usenet.Nntp.Models;

namespace Spottarr.Services;

internal sealed class SpotnetService : ISpotnetService
{
    private readonly ILogger<SpotnetService> _logger;
    private readonly IOptions<UsenetOptions> _usenetOptions;
    private readonly IOptions<SpotnetOptions> _spotnetOptions;

    public SpotnetService(ILoggerFactory loggerFactory, ILogger<SpotnetService> logger,
        IOptions<UsenetOptions> usenetOptions, IOptions<SpotnetOptions> spotnetOptions)
    {
        _logger = logger;
        _usenetOptions = usenetOptions;
        _spotnetOptions = spotnetOptions;

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
        var spots = GetSpots(group, client).ToList();
        
        client.Quit();
    }

    private IEnumerable<string> GetSpots(NntpGroup group, NntpClient client)
    {
        var batches = GetBatches(group.HighWaterMark - 5000, group.HighWaterMark).ToList();

        foreach (var batch in batches)
        {
            var xoverResponse = client.Xover(batch);
            if(!xoverResponse.Success)
            {
                _logger.CouldNotRetrieveArticles(batch.From, batch.To, xoverResponse.Code, xoverResponse.Message);
                continue;
            }

            foreach (var header in xoverResponse.Lines)
            {
                if (header == null) continue;
                yield return header;
            }
        }
    }

    private static IEnumerable<NntpArticleRange> GetBatches(long lowWaterMark, long highWaterMark)
    {
        var batchCount = 0; 
        for (var i = highWaterMark; i >= lowWaterMark; i--)
        {
            batchCount++;
            if (batchCount != 1000) continue;
            
            var range = new NntpArticleRange(highWaterMark - batchCount, highWaterMark);
            highWaterMark -= batchCount;
            batchCount = 0;
            yield return range;
        }
        
        yield return new NntpArticleRange(highWaterMark - batchCount, highWaterMark);
    }
}