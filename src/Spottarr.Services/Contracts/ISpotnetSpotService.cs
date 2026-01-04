using Spottarr.Configuration.Options;
using Spottarr.Data.Entities;
using Usenet.Nntp.Models;

namespace Spottarr.Services.Contracts;

public interface ISpotnetSpotService
{
    Task<IReadOnlyList<Spot>> FetchSpotHeaders(SpotnetOptions options, NntpArticleRange batch,
        CancellationToken cancellationToken);

    ValueTask GetSpotDetails(Spot spot, CancellationToken cancellationToken);
}