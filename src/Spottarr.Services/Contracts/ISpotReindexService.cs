using Spottarr.Services.Models;

namespace Spottarr.Services.Contracts;

public interface ISpotReindexService
{
    /// <summary>
    /// Derives the indexable attributes of the spots that are marked for a reindex.
    /// </summary>
    Task Reindex(CancellationToken cancellationToken);

    /// <summary>
    /// Marks the selected spots to be indexed again and returns how many are marked.
    /// </summary>
    Task<int> MarkForReindex(SpotSelection selection, CancellationToken cancellationToken);

    /// <summary>
    /// Unmarks the spots that are waiting to be indexed again and returns how many were unmarked.
    /// </summary>
    Task<int> UnmarkForReindex(CancellationToken cancellationToken);

    Task<int> CountMarkedForReindex(CancellationToken cancellationToken);
}
