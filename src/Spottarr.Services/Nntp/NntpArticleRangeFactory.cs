using Usenet.Nntp.Models;

namespace Spottarr.Services.Nntp;

internal sealed class NntpArticleRangeFactory
{
    /// <summary>
    /// Returns a sequence of article ranges ordered from the batch with the newest messages to the oldest messages
    /// </summary>
    /// <param name="lowWaterMark">The oldest message sequence number to include in the ranges</param>
    /// <param name="highWaterMark">The newest message sequence number to include in the ranges</param>
    /// <param name="importBatchSize"></param>
    public static IReadOnlyList<NntpArticleRange> GetBatches(long lowWaterMark, long highWaterMark, int importBatchSize) =>
        GetBatchesInternal(lowWaterMark, highWaterMark, importBatchSize).ToList();
    
    private static IEnumerable<NntpArticleRange> GetBatchesInternal(long min, long max, int size)
    {
        for (var start = min; start < max; start += size)
        {
            var inclusiveOffset = start + size < max ? -1 : 0;
            var end = Math.Min(start + size + inclusiveOffset, max);
            yield return new NntpArticleRange(start, end);
        }
    }
}