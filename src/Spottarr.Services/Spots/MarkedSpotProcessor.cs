using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Spottarr.Data;
using Spottarr.Data.Entities;
using Spottarr.Services.Models;

namespace Spottarr.Services.Spots;

/// <summary>
/// Marks spots for reprocessing and processes the marked spots in batches.
/// A spot is marked while the timestamp of the operation is unset.
/// </summary>
internal abstract class MarkedSpotProcessor
{
    private readonly IDbContextFactory<SpottarrDbContext> _dbContextFactory;

    protected MarkedSpotProcessor(IDbContextFactory<SpottarrDbContext> dbContextFactory) =>
        _dbContextFactory = dbContextFactory;

    protected abstract int BatchSize { get; }

    /// <summary>
    /// Selects the spots that are waiting to be processed.
    /// </summary>
    protected abstract Expression<Func<Spot, bool>> IsMarked { get; }

    /// <summary>
    /// The timestamp that is unset while a spot is marked.
    /// </summary>
    protected abstract Expression<Func<Spot, DateTime?>> ProcessedAt { get; }

    protected abstract Task ProcessBatch(
        SpottarrDbContext dbContext,
        IReadOnlyList<Spot> spots,
        CancellationToken cancellationToken
    );

    protected abstract void LogStarted();
    protected abstract void LogFinished();
    protected abstract void LogBatchStarted(int current, int total);
    protected abstract void LogBatchFinished(int current, int total, int spotCount);

    protected async Task ProcessMarkedSpots(CancellationToken cancellationToken)
    {
        LogStarted();

        var markedCount = await CountMarked(cancellationToken);
        if (markedCount > 0)
        {
            // Counted up front so the log lines show progress. Spots that are unmarked while we are
            // running simply make the next batch come back empty.
            var batchCount = (markedCount / BatchSize) + 1;
            var lastProcessedId = 0;

            for (var i = 0; i < batchCount; i++)
            {
                LogBatchStarted(i + 1, batchCount);

                var spots = await ProcessNextBatch(lastProcessedId, cancellationToken);
                if (spots.Count == 0)
                    break;

                // Spots that stay marked because of a temporary failure are picked up by the next
                // run instead of blocking the rest of this one.
                lastProcessedId = spots[^1].Id;

                LogBatchFinished(i + 1, batchCount, spots.Count);
            }
        }

        LogFinished();
    }

    /// <summary>
    /// Marks the selected spots by unsetting the timestamp of the operation.
    /// </summary>
    protected async Task<int> Mark(SpotSelection selection, CancellationToken cancellationToken)
    {
        ArgumentNullException.ThrowIfNull(selection);

        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);

        var spots = dbContext.Spots.AsQueryable();

        if (selection.SpotIds is { Count: > 0 } spotIds)
            spots = spots.Where(s => spotIds.Contains(s.Id));

        if (selection.SpottedAfter is { } spottedAfter)
            spots = spots.Where(s => s.SpottedAt >= spottedAfter.UtcDateTime);

        if (selection.SpottedBefore is { } spottedBefore)
            spots = spots.Where(s => s.SpottedAt <= spottedBefore.UtcDateTime);

        return await spots.ExecuteUpdateAsync(
            setters => setters.SetProperty(ProcessedAt, (DateTime?)null),
            cancellationToken
        );
    }

    /// <summary>
    /// Unmarks every marked spot by stamping it as processed.
    /// </summary>
    protected async Task<int> Unmark(CancellationToken cancellationToken)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var now = DateTimeOffset.Now.UtcDateTime;

        return await dbContext
            .Spots.Where(IsMarked)
            .ExecuteUpdateAsync(
                setters => setters.SetProperty(ProcessedAt, now),
                cancellationToken
            );
    }

    protected async Task<int> CountMarked(CancellationToken cancellationToken)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await dbContext.Spots.Where(IsMarked).CountAsync(cancellationToken);
    }

    private async Task<IReadOnlyList<Spot>> ProcessNextBatch(
        int lastProcessedId,
        CancellationToken cancellationToken
    )
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var spots = await dbContext
            .Spots.Where(IsMarked)
            .Where(s => s.Id > lastProcessedId)
            .AsNoTracking()
            .OrderBy(s => s.Id)
            .Take(BatchSize)
            .ToListAsync(cancellationToken);

        if (spots.Count > 0)
            await ProcessBatch(dbContext, spots, cancellationToken);

        return spots;
    }
}
