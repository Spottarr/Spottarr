using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PhenX.EntityFrameworkCore.BulkInsert.Extensions;
using PhenX.EntityFrameworkCore.BulkInsert.Options;
using Spottarr.Data;
using Spottarr.Data.Entities;
using Spottarr.Services.Configuration;
using Spottarr.Services.Contracts;
using Spottarr.Services.Helpers;
using Spottarr.Services.Logging;
using Spottarr.Services.Newznab;
using Spottarr.Services.Parsers;

namespace Spottarr.Services;

/// <summary>
/// Extracts useful attributes from spots and cleans up their title and description
/// </summary>
internal sealed class SpotIndexingService : ISpotIndexingService
{
    private readonly ILogger<SpotIndexingService> _logger;
    private readonly SpottarrDbContext _dbContext;
    private readonly IOptions<SpotnetOptions> _spotnetOptions;

    public SpotIndexingService(ILogger<SpotIndexingService> logger, SpottarrDbContext dbContext,
        IOptions<SpotnetOptions> spotnetOptions)
    {
        _logger = logger;
        _dbContext = dbContext;
        _spotnetOptions = spotnetOptions;
    }

    public async Task Index(CancellationToken cancellationToken)
    {
        _logger.SpotIndexingStarted(DateTimeOffset.Now);

        var options = _spotnetOptions.Value;
        var unIndexedSpotsCount = await _dbContext.Spots.Where(s => s.IndexedAt == null).CountAsync(cancellationToken);

        if (unIndexedSpotsCount > 0)
        {
            var batchCount = (unIndexedSpotsCount / options.ImportBatchSize) + 1;

            for (var i = 0; i < batchCount; i++)
            {
                _logger.SpotIndexingBatchStarted(i + 1, batchCount, DateTimeOffset.Now);

                var indexedSpots = await IndexBatch(options.ImportBatchSize, cancellationToken);

                _logger.SpotIndexingBatchFinished(i + 1, batchCount, DateTimeOffset.Now, indexedSpots);
            }
        }

        _logger.SpotIndexingFinished(DateTimeOffset.Now);
    }

    private async Task<int> IndexBatch(int batchSize, CancellationToken cancellationToken)
    {
        var unIndexedSpots = await _dbContext.Spots.Where(s => s.IndexedAt == null)
            .AsNoTracking()
            .OrderBy(s => s.Id)
            .Take(batchSize)
            .ToListAsync(cancellationToken);

        if (unIndexedSpots.Count == 0) return unIndexedSpots.Count;

        var now = DateTimeOffset.Now.UtcDateTime;
        var fullTextIndexSpots = new List<FtsSpot>();

        foreach (var spot in unIndexedSpots)
        {
            spot.Description = BbCodeParser.Parse(spot.Description);

            var titleAndDescription = string.Join('\n', spot.Title, spot.Description);
            var (years, seasons, episodes) = YearEpisodeSeasonParser.Parse(titleAndDescription);

            spot.Years.Replace(years);
            spot.Seasons.Replace(seasons);
            spot.Episodes.Replace(episodes);
            spot.NewznabCategories.Replace(NewznabCategoryMapper.Map(spot));
            spot.ImdbId = ImdbIdParser.Parse(spot.Url);
            spot.ReleaseTitle = ReleaseTitleParser.Parse(titleAndDescription);
            spot.IndexedAt = now;
            spot.UpdatedAt = now;

            var ftsSpot = new FtsSpot
            {
                RowId = spot.Id,
                Title = FtsTitleParser.Parse(spot.Title),
                Description = spot.Description
            };

            fullTextIndexSpots.Add(ftsSpot);
        }

        var fullTextIndexSpotIds = fullTextIndexSpots
            .Select(s => s.RowId).ToHashSet();

        try
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync(cancellationToken);

            // EF does not natively support FTS tables, so we have to delete and re-insert the records in case any already exist
            await _dbContext.FtsSpots
                .Where(f => fullTextIndexSpotIds.Contains(f.RowId))
                .ExecuteDeleteAsync(cancellationToken);

            await _dbContext.ExecuteBulkInsertAsync(fullTextIndexSpots, cancellationToken: cancellationToken);
            await _dbContext.ExecuteBulkInsertAsync(unIndexedSpots, new OnConflictOptions<Spot>
            {
                Update = (existing, inserted) => new Spot
                {
                    ReleaseTitle = inserted.ReleaseTitle,
                    Years = inserted.Years,
                    Seasons = inserted.Seasons,
                    Episodes = inserted.Episodes,
                    NewznabCategories = inserted.NewznabCategories,
                    ImdbId = inserted.ImdbId,
                    TvdbId = inserted.TvdbId,
                    IndexedAt = inserted.IndexedAt,
                    UpdatedAt = inserted.UpdatedAt,
                }
            }, cancellationToken);
            await transaction.CommitAsync(cancellationToken);
        }
        catch (DbException ex)
        {
            _logger.FailedToSaveSpots(ex);
        }

        return unIndexedSpots.Count;
    }
}