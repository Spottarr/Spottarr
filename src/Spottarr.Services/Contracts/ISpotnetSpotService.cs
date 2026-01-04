using Spottarr.Data.Entities;
using Usenet.Nntp.Models;

namespace Spottarr.Services.Contracts;

public interface ISpotnetSpotService
{
    Task<IReadOnlyList<Spot>> FetchSpotHeaders(NntpArticleRange batch, CancellationToken cancellationToken);

    Task<IReadOnlyList<Spot>> FetchSpotDetails(IReadOnlyList<Spot> spots, int maxDegreeOfParallelism,
        CancellationToken cancellationToken);
}