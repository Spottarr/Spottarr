using Spottarr.Services.Models;

namespace Spottarr.Services.Contracts;

public interface ISpotSearchService
{
    Task<SpotSearchResponse> Search(SpotSearchFilter filter, CancellationToken cancellationToken);
    Task<int> Count(CancellationToken cancellationToken);
}