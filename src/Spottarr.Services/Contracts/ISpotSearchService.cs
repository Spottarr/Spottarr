namespace Spottarr.Services.Contracts;

public interface ISpotSearchService
{
    Task<SpotSearchResponse> Search(SpotSearchFilter filter);
}