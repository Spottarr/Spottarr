namespace Spottarr.Web.Api.Models;

internal sealed record MarkedSpotsResponse
{
    public required int Marked { get; init; }
}
