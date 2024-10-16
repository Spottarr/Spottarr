using Spottarr.Data.Entities;

namespace Spottarr.Services.Helpers;

internal sealed class SpotImportResult
{
    private readonly HashSet<string> _messageIds;
    public List<ImageSpot> ImageSpots { get; } = [];
    public List<AudioSpot> AudioSpots { get; } = [];
    public List<GameSpot> GameSpots { get; } = [];
    public List<ApplicationSpot> ApplicationSpots { get; } = [];

    public SpotImportResult(HashSet<string> messageIds) => _messageIds = messageIds;

    public void AddSpot(Spot spot)
    {
        // Deduplicate on message ID
        if (!_messageIds.Add(spot.MessageId)) return;

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
}