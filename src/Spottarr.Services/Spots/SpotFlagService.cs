using Microsoft.EntityFrameworkCore;
using Spottarr.Data;
using Spottarr.Data.Entities;
using Spottarr.Services.Contracts;
using Spottarr.Services.Models;

namespace Spottarr.Services.Spots;

/// <summary>
/// Flags spots for a reimport or reindex. The flag is the absence of the matching timestamp,
/// so setting it is a single update over the selected spots.
/// </summary>
internal sealed class SpotFlagService : ISpotFlagService
{
    private readonly IDbContextFactory<SpottarrDbContext> _dbContextFactory;

    public SpotFlagService(IDbContextFactory<SpottarrDbContext> dbContextFactory) =>
        _dbContextFactory = dbContextFactory;

    public async Task<int> Flag(
        SpotOperation operation,
        SpotSelection selection,
        CancellationToken cancellationToken
    )
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

        return operation switch
        {
            SpotOperation.Reimport => await spots.ExecuteUpdateAsync(
                s => s.SetProperty(x => x.ImportedAt, (DateTime?)null),
                cancellationToken
            ),
            SpotOperation.Reindex => await spots.ExecuteUpdateAsync(
                s => s.SetProperty(x => x.IndexedAt, (DateTime?)null),
                cancellationToken
            ),
            _ => throw new ArgumentOutOfRangeException(nameof(operation)),
        };
    }

    public async Task<int> ClearFlags(SpotOperation operation, CancellationToken cancellationToken)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var now = DateTimeOffset.Now.UtcDateTime;

        // Flagged spots are stamped as processed, which is all the flag records.
        return operation switch
        {
            SpotOperation.Reimport => await Flagged(dbContext, operation)
                .ExecuteUpdateAsync(s => s.SetProperty(x => x.ImportedAt, now), cancellationToken),
            SpotOperation.Reindex => await Flagged(dbContext, operation)
                .ExecuteUpdateAsync(s => s.SetProperty(x => x.IndexedAt, now), cancellationToken),
            _ => throw new ArgumentOutOfRangeException(nameof(operation)),
        };
    }

    public async Task<int> CountFlagged(
        SpotOperation operation,
        CancellationToken cancellationToken
    )
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        return await Flagged(dbContext, operation).CountAsync(cancellationToken);
    }

    private static IQueryable<Spot> Flagged(SpottarrDbContext dbContext, SpotOperation operation) =>
        operation switch
        {
            SpotOperation.Reimport => dbContext.Spots.Where(s => s.ImportedAt == null),
            SpotOperation.Reindex => dbContext.Spots.Where(s => s.IndexedAt == null),
            _ => throw new ArgumentOutOfRangeException(nameof(operation)),
        };
}
