using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Spottarr.Data;
using Spottarr.Data.Entities;
using Spottarr.Services.Logging;

namespace Spottarr.Services.Spots;

/// <summary>
/// Processes spots that are flagged for reprocessing in batches until no flagged spots remain.
/// </summary>
internal abstract class SpotFlagDrainService
{
    private readonly ILogger _logger;
    private readonly IDbContextFactory<SpottarrDbContext> _dbContextFactory;

    protected SpotFlagDrainService(
        ILogger logger,
        IDbContextFactory<SpottarrDbContext> dbContextFactory
    )
    {
        _logger = logger;
        _dbContextFactory = dbContextFactory;
    }

    protected abstract string Operation { get; }
    protected abstract int BatchSize { get; }
    protected abstract Expression<Func<Spot, bool>> IsFlagged { get; }

    protected abstract Task ProcessBatch(
        SpottarrDbContext dbContext,
        IReadOnlyList<Spot> spots,
        CancellationToken cancellationToken
    );

    protected async Task Drain(CancellationToken cancellationToken)
    {
        _logger.SpotDrainStarted(Operation, DateTimeOffset.Now);

        var flaggedCount = await CountFlagged(cancellationToken);
        if (flaggedCount > 0)
        {
            // Counted up front so the log lines show progress, but the flags are the actual work
            // list: clearing them stops the drain at the next batch.
            var batchCount = (flaggedCount / BatchSize) + 1;
            var cursor = 0;

            for (var i = 0; i < batchCount; i++)
            {
                _logger.SpotDrainBatchStarted(Operation, i + 1, batchCount, DateTimeOffset.Now);

                var spots = await ProcessNextBatch(cursor, cancellationToken);
                if (spots.Count == 0)
                    break;

                // Spots that keep their flag because of a transient failure are retried on the next
                // run instead of blocking the rest of this one.
                cursor = spots[^1].Id;

                _logger.SpotDrainBatchFinished(
                    Operation,
                    i + 1,
                    batchCount,
                    DateTimeOffset.Now,
                    spots.Count
                );
            }
        }

        _logger.SpotDrainFinished(Operation, DateTimeOffset.Now);
    }

    private async Task<int> CountFlagged(CancellationToken cancellationToken)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await dbContext.Spots.Where(IsFlagged).CountAsync(cancellationToken);
    }

    private async Task<IReadOnlyList<Spot>> ProcessNextBatch(
        int cursor,
        CancellationToken cancellationToken
    )
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var spots = await dbContext
            .Spots.Where(IsFlagged)
            .Where(s => s.Id > cursor)
            .AsNoTracking()
            .OrderBy(s => s.Id)
            .Take(BatchSize)
            .ToListAsync(cancellationToken);

        if (spots.Count > 0)
            await ProcessBatch(dbContext, spots, cancellationToken);

        return spots;
    }
}
