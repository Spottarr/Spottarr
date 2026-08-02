using Spottarr.Data.Entities;
using Spottarr.Services.Spotnet;
using Usenet.Nntp.Models;

namespace Spottarr.Services.Contracts;

public interface ISpotnetSpotService
{
    Task<IReadOnlyList<Spot>> FetchSpotHeaders(
        NntpArticleRange batch,
        CancellationToken cancellationToken
    );

    Task FetchSpotDetails(
        IReadOnlyList<Spot> spots,
        int maxDegreeOfParallelism,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Rereads the article of an already imported spot and overwrites its attributes in place.
    /// </summary>
    Task<SpotReadOutcome> RereadSpot(Spot spot, CancellationToken cancellationToken);
}
