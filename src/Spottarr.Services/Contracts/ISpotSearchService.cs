using Spottarr.Services.Models;

namespace Spottarr.Services.Contracts;

public interface ISpotSearchService
{
    Task<SpotSearchResponse> Search(SpotSearchFilter filter);
    Task<int> Count();
}