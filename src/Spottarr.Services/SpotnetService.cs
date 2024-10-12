using Spottarr.Services.Configuration;
using Spottarr.Services.Contracts;

namespace Spottarr.Services;

internal sealed class SpotnetService : ISpotnetService
{
    private readonly UsenetOptions _usenetOptions;
    
    public SpotnetService(UsenetOptions usenetOptions)
    {
        _usenetOptions = usenetOptions;
    }

    public Task Import()
    {
        throw new NotImplementedException();
    }
}