using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spottarr.Services.Configuration;
using Spottarr.Services.Contracts;
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
        var range = new NntpArticleRange(group.HighWaterMark, null);
        
        var articleResponse = client.Xover(range);
        if (!articleResponse.Success)
        {
            _logger.CouldNotRetrieveArticles(range.From, range.To, spotnetOptions.SpotGroup, groupResponse.Code, groupResponse.Message);
            client.Quit();
            return;
        }

        client.Quit();
    }
}