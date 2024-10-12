using Microsoft.Extensions.Options;
using Spottarr.Services.Configuration;
using Spottarr.Services.Contracts;

namespace Spottarr.Services;

internal sealed class SpotnetService : ISpotnetService
{
    private readonly IOptions<UsenetOptions> _usenetOptions;
    
    public SpotnetService(IOptions<UsenetOptions> usenetOptions) => _usenetOptions = usenetOptions;

    public Task Import()
    {
        throw new NotImplementedException();
    }
}