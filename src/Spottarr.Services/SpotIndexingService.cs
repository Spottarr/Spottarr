using System.Data.Common;
using System.Text.RegularExpressions;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Spottarr.Data;
using Spottarr.Data.Entities;
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

    public SpotIndexingService(ILogger<SpotIndexingService> logger, SpottarrDbContext dbContext)
    {
        _logger = logger;
        _dbContext = dbContext;
    }

    public async Task Index()
    {
        _logger.SpotIndexingStarted(DateTimeOffset.Now);

        var unIndexedSpots = await _dbContext.Spots.Where(s => s.IndexedAt == null)
            .AsNoTracking()
            .ToListAsync();

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

        _logger.SpotIndexingFinished(DateTimeOffset.Now, fullTextIndexSpots.Count);
    }

    [GeneratedRegex(@"(?<=\w)\.(?=\w)")]
    private static partial Regex CleanTitleRegex();
    
    [GeneratedRegex(@"\b(\w+\.)+\w+-\w+\b", RegexOptions.IgnoreCase)]
    private static partial Regex ReleaseTitleRegex();
}