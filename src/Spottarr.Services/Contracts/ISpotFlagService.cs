using Spottarr.Services.Models;

namespace Spottarr.Services.Contracts;

public interface ISpotFlagService
{
    /// <summary>
    /// Flags the selected spots and returns how many are flagged.
    /// </summary>
    Task<int> Flag(
        SpotOperation operation,
        SpotSelection selection,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Clears the flag on all flagged spots and returns how many were cleared.
    /// </summary>
    Task<int> ClearFlags(SpotOperation operation, CancellationToken cancellationToken);

    /// <summary>
    /// Counts the spots that carry the flag.
    /// </summary>
    Task<int> CountFlagged(SpotOperation operation, CancellationToken cancellationToken);
}
