using Spottarr.Services.Models;

namespace Spottarr.Services.Contracts;

public interface ISpotReimportService
{
    /// <summary>
    /// Rereads the articles of the spots that are marked for a reimport.
    /// </summary>
    Task Reimport(CancellationToken cancellationToken);

    /// <summary>
    /// Marks the selected spots to be reread from usenet and returns how many are marked.
    /// </summary>
    Task<int> MarkForReimport(SpotSelection selection, CancellationToken cancellationToken);

    /// <summary>
    /// Unmarks the spots that are waiting to be reread and returns how many were unmarked.
    /// </summary>
    Task<int> UnmarkForReimport(CancellationToken cancellationToken);

    Task<int> CountMarkedForReimport(CancellationToken cancellationToken);
}
