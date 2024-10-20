using Spottarr.Data;
using Spottarr.Data.Entities;
using Spottarr.Services.Nntp;

namespace Spottarr.Services.Helpers;

internal sealed class SpotImportResult
{
    private readonly HashSet<string> _existingMessageIds;
    public List<Spot> Spots { get; } = [];
    public List<ImageSpot> ImageSpots { get; } = [];
    public List<AudioSpot> AudioSpots { get; } = [];
    public List<GameSpot> GameSpots { get; } = [];
    public List<ApplicationSpot> ApplicationSpots { get; } = [];
    public HashSet<string> DeletedSpots { get; } = new(StringComparer.OrdinalIgnoreCase);

    public SpotImportResult(HashSet<string> existingMessageIds) => _existingMessageIds = existingMessageIds;

    public void AddSpot(Spot spot)
    {
        // Deduplicate on message ID
        if (!_existingMessageIds.Add(spot.MessageId)) return;

        Spots.Add(spot);

        Action action = spot switch
        {
            ImageSpot imageSpot => () => ImageSpots.Add(imageSpot),
            AudioSpot audioSpot => () => AudioSpots.Add(audioSpot),
            GameSpot gameSpot => () => GameSpots.Add(gameSpot),
            ApplicationSpot applicationSpot => () => ApplicationSpots.Add(applicationSpot),
            _ => throw new ArgumentOutOfRangeException(nameof(spot), spot, null)
        };
        
        action.Invoke();
    }

    public void AddDeletion(SpotHeader spotnetHeader) => DeletedSpots.Add(spotnetHeader.NntpHeader.MessageId);
}