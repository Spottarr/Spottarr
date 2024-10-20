using System.Globalization;
using System.Text.RegularExpressions;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Spottarr.Data;
using Spottarr.Data.Entities;
using Spottarr.Services.Contracts;
using Spottarr.Services.Helpers;
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
        var unindexedSpots = await _dbContext.Spots.Where(s => s.IndexedAt == null)
            .ToListAsync();

        var now = DateTimeOffset.Now;
        var fullTextIndexSpots = new List<FtsSpot>();
        
        foreach (var spot in unindexedSpots)
        {
            var title = spot.Title;
            
            // Replace BB tags
            var description = (spot.Description ?? string.Empty)
                .Replace("[br]", "\n", StringComparison.OrdinalIgnoreCase);
            
            // Clean up title
            // e.g. "Show.S01E04.Poster.1080p.DDP5.1.Atmos.H.264" -> "Show S01E04 Poster 1080p DDP5 1 Atmos H 264"
            title = CleanTitleRegex()
                .Replace(spot.Title, " ");

            var titleAndDescription = string.Join('\n', title, description);

            // Search for year, season and episode numbers.
            // e.g. "2024 S01E04", "Season: 1", "Episode 2"
            // We store all values found to make it easier to search for them
            var (years, episodes, seasons) = YearEpisodeSeasonParser.Parse(titleAndDescription);

            spot.Title = title;
            spot.Description = description;
            spot.Years.Replace(years);
            spot.Seasons.Replace(seasons);
            spot.Episodes.Replace(episodes);
            spot.IndexedAt = now.UtcDateTime;

            var ftsSpot = new FtsSpot()
            {
                RowId = spot.Id,
                Title = title,
                Description = description
            };
            
            fullTextIndexSpots.Add(ftsSpot);
        }

        await _dbContext.BulkUpdateAsync(unindexedSpots, c =>
        {
            c.PropertiesToIncludeOnUpdate =
            [
                nameof(Spot.Title),
                nameof(Spot.Description),
                nameof(Spot.Years),
                nameof(Spot.Seasons),
                nameof(Spot.Episodes),
                nameof(Spot.IndexedAt),
            ];
        });

        await _dbContext.BulkInsertAsync(fullTextIndexSpots);
    }
    
    [GeneratedRegex(@"(?<=\w)\.(?=\w)")]
    private static partial Regex CleanTitleRegex();
}