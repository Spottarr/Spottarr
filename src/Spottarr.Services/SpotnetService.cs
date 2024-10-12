using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spottarr.Services.Configuration;
using Spottarr.Services.Contracts;
using Usenet.Nntp;

namespace Spottarr.Services;

internal sealed class SpotnetService : ISpotnetService
{
    private readonly IOptions<UsenetOptions> _usenetOptions;
    private readonly IOptions<SpotnetOptions> _spotnetOptions;

    public SpotnetService(ILoggerFactory loggerFactory, IOptions<UsenetOptions> usenetOptions, IOptions<SpotnetOptions> spotnetOptions)
    {
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
        client.Group(spotnetOptions.SpotGroup);
    }
}