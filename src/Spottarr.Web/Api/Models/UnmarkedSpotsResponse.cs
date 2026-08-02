namespace Spottarr.Web.Api.Models;

internal sealed record UnmarkedSpotsResponse
{
    public required int Unmarked { get; init; }
}
