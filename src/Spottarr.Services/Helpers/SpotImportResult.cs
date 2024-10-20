using System.Collections.Concurrent;
using Spottarr.Data.Entities;
using Spottarr.Services.Spotnet;

namespace Spottarr.Services.Helpers;

internal sealed class SpotImportResult
{
    private readonly HashSet<string> _existingMessageIds;
    public List<Spot> Spots { get; } = [];
    public HashSet<string> DeletedSpots { get; } = new(StringComparer.OrdinalIgnoreCase);

    public SpotImportResult(HashSet<string> existingMessageIds) => _existingMessageIds = existingMessageIds;

    public void AddSpot(Spot spot)
    {
        // Deduplicate on message ID
        if (!_existingMessageIds.Add(spot.MessageId)) return;

        Spots.Add(spot);
    }

    public void AddDeletion(SpotHeader spotnetHeader) => DeletedSpots.Add(spotnetHeader.NntpHeader.MessageId);
}