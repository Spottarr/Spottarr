using System.Data.Common;
using System.Text.RegularExpressions;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
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
internal sealed partial class SpotIndexingService : ISpotIndexingService
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

    public async Task Index()
    {
        _logger.SpotIndexingStarted(DateTimeOffset.Now);

        var options = _spotnetOptions.Value;
        var unIndexedSpotsCount = await _dbContext.Spots.Where(s => s.IndexedAt == null).CountAsync();

        if (unIndexedSpotsCount > 0)
        {
            var batchCount = (unIndexedSpotsCount / options.ImportBatchSize) + 1;

            for (var i = 0; i < batchCount; i++)
            {
                _logger.SpotIndexingBatchStarted(i + 1, batchCount, DateTimeOffset.Now);

                var indexedSpots = await IndexBatch(options.ImportBatchSize);

                _logger.SpotIndexingBatchFinished(i + 1, batchCount, DateTimeOffset.Now, indexedSpots);
            }
        }

        _logger.SpotIndexingFinished(DateTimeOffset.Now);
    }

    private async Task<int> IndexBatch(int batchSize)
    {
        var unIndexedSpots = await _dbContext.Spots.Where(s => s.IndexedAt == null)
            .AsNoTracking()
            .OrderBy(s => s.Id)
            .Take(batchSize)
            .ToListAsync();

        if (unIndexedSpots.Count == 0) return unIndexedSpots.Count;

        var now = DateTimeOffset.Now;
        var fullTextIndexSpots = new List<FtsSpot>();
        var releaseTitleRegex = ReleaseTitleRegex();

        foreach (var spot in unIndexedSpots)
        {
            // Replace BB tags
            var description = (spot.Description ?? string.Empty)
                .Replace("[br]", "\n", StringComparison.OrdinalIgnoreCase);

            var titleAndDescription = string.Join('\n', spot.Title, description);

            // Extract release title
            var releaseMatch = releaseTitleRegex.Match(titleAndDescription);
            if (releaseMatch.Success)
                spot.ReleaseTitle = releaseMatch.Value;

            // Search for year, season and episode numbers.
            // e.g. "2024 S01E04", "Season: 1", "Episode 2"
            // We store all values found to make it easier to search for them
            var (years, seasons, episodes) = YearEpisodeSeasonParser.Parse(titleAndDescription);

            var newznabCategories = NewznabCategoryMapper.Map(spot);

            spot.Years.Replace(years);
            spot.Seasons.Replace(seasons);
            spot.Episodes.Replace(episodes);
            spot.NewznabCategories.Replace(newznabCategories);
            spot.ImdbId = ImdbIdParser.Parse(spot.Url);
            spot.IndexedAt = now.UtcDateTime;
            spot.UpdatedAt = now.UtcDateTime;

            // Clean up release title for fulltext search
            // e.g. "Show.S01E04.Poster.1080p.DDP5.1.Atmos.H.264" -> "Show S01E04 Poster 1080p DDP5 1 Atmos H 264"
            var ftsTitle = CleanTitleRegex()
                .Replace(spot.Title, " ");

            var ftsSpot = new FtsSpot()
            {
                RowId = spot.Id,
                Title = ftsTitle,
                Description = description
            };

            fullTextIndexSpots.Add(ftsSpot);
        }

        try
        {
            await using var transaction = await _dbContext.Database.BeginTransactionAsync();
            await _dbContext.BulkInsertAsync(fullTextIndexSpots, progress: p => _logger.BulkInsertUpdateProgress(p));
            await _dbContext.BulkUpdateAsync(unIndexedSpots, c =>
            {
                c.PropertiesToIncludeOnUpdate =
                [
                    nameof(Spot.Title),
                    nameof(Spot.ReleaseTitle),
                    nameof(Spot.Description),
                    nameof(Spot.Years),
                    nameof(Spot.Seasons),
                    nameof(Spot.Episodes),
                    nameof(Spot.NewznabCategories),
                    nameof(Spot.IndexedAt),
                    nameof(Spot.UpdatedAt)
                ];
            }, progress: p => _logger.BulkInsertUpdateProgress(p));
            await transaction.CommitAsync();
        }
        catch (DbException ex)
        {
            _logger.FailedToSaveSpots(ex);
        }

        return unIndexedSpots.Count;
    }

    [GeneratedRegex(@"(?<=\w)\.(?=\w)")]
    private static partial Regex CleanTitleRegex();

    [GeneratedRegex(@"\b(\w+\.)+\w+-\w+\b", RegexOptions.IgnoreCase)]
    private static partial Regex ReleaseTitleRegex();
}