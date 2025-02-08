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
    
    private static IEnumerable<NntpArticleRange> GetBatchesInternal(long lowWaterMark, long highWaterMark, int importBatchSize)
    {
        var batchEnd = highWaterMark;
        while (batchEnd >= lowWaterMark)
        {
            var batchStart = GetBatchStart(lowWaterMark, batchEnd, importBatchSize);
            var range = new NntpArticleRange(batchStart, batchEnd);
            batchEnd = batchStart - 1;
            yield return range;
        }
    }

    private static long GetBatchStart(long lowWaterMark, long batchEnd, int importBatchSize)
    {
        var batchStart = Math.Max(lowWaterMark, batchEnd - (importBatchSize - 1));

        // Make sure that the final batch is inclusive
        if (batchStart - 1 == lowWaterMark) batchStart = lowWaterMark;
        return batchStart;
    }
}