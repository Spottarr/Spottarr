namespace Spottarr.Web.Api.Models;

internal sealed record MarkedSpotsStatusResponse
{
    public required int Marked { get; init; }

    /// <summary>
    /// Whether the import job that processes marked spots is running.
    /// </summary>
    public required bool Running { get; init; }
}
