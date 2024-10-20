using Spottarr.Data.Entities;

namespace Spottarr.Services.Models;

public class SpotSearchResponse
{
    public required int TotalCount { get; init; }
    public required ICollection<Spot> Spots { get; init; }
}