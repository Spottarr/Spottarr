using System.Collections.Concurrent;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PhenX.EntityFrameworkCore.BulkInsert.Extensions;
using PhenX.EntityFrameworkCore.BulkInsert.Options;
using Spottarr.Configuration.Options;
using Spottarr.Data;
using Spottarr.Data.Entities;
using Spottarr.Services.Contracts;
using Spottarr.Services.Logging;
using Spottarr.Services.Models;
using Spottarr.Services.Spotnet;

namespace Spottarr.Services.Spots;

/// <summary>
/// Rereads the articles of spots that are marked for a reimport and overwrites them in place.
/// </summary>
internal sealed class SpotReimportService : MarkedSpotProcessor, ISpotReimportService
{
    /// <summary>
    /// Rereading costs one usenet request per spot, so batches are kept small enough to stop quickly
    /// when the spots are unmarked halfway through.
    /// </summary>
    private const int ReimportBatchSize = 500;

    private readonly ILogger<SpotReimportService> _logger;
    private readonly IOptions<UsenetOptions> _usenetOptions;
    private readonly ISpotnetSpotService _spotnetSpotService;

    public SpotReimportService(
        ILogger<SpotReimportService> logger,
        IDbContextFactory<SpottarrDbContext> dbContextFactory,
        IOptions<UsenetOptions> usenetOptions,
        ISpotnetSpotService spotnetSpotService
    )
        : base(dbContextFactory)
    {
        _logger = logger;
        _usenetOptions = usenetOptions;
        _spotnetSpotService = spotnetSpotService;
    }

    protected override int BatchSize => ReimportBatchSize;
    protected override Expression<Func<Spot, bool>> IsMarked => s => s.ImportedAt == null;
    protected override Expression<Func<Spot, DateTime?>> ProcessedAt => s => s.ImportedAt;

    public Task Reimport(CancellationToken cancellationToken) =>
        ProcessMarkedSpots(cancellationToken);

    public Task<int> MarkForReimport(
        SpotSelection selection,
        CancellationToken cancellationToken
    ) => Mark(selection, cancellationToken);

    public Task<int> UnmarkForReimport(CancellationToken cancellationToken) =>
        Unmark(cancellationToken);

    public Task<int> CountMarkedForReimport(CancellationToken cancellationToken) =>
        CountMarked(cancellationToken);

    protected override void LogStarted() => _logger.SpotReimportStarted(DateTimeOffset.Now);

    protected override void LogFinished() => _logger.SpotReimportFinished(DateTimeOffset.Now);

    protected override void LogBatchStarted(int current, int total) =>
        _logger.SpotReimportBatchStarted(current, total, DateTimeOffset.Now);

    protected override void LogBatchFinished(int current, int total, int spotCount) =>
        _logger.SpotReimportBatchFinished(current, total, DateTimeOffset.Now, spotCount);

    protected override async Task ProcessBatch(
        SpottarrDbContext dbContext,
        IReadOnlyList<Spot> spots,
        CancellationToken cancellationToken
    )
    {
        var reread = new ConcurrentBag<Spot>();
        var unavailable = new ConcurrentBag<int>();

        var parallelOptions = new ParallelOptions
        {
            MaxDegreeOfParallelism = _usenetOptions.Value.MaxConnections,
            CancellationToken = cancellationToken,
        };

        await Parallel.ForEachAsync(
            spots,
            parallelOptions,
            async (spot, ct) =>
            {
                switch (await _spotnetSpotService.RereadSpot(spot, ct))
                {
                    case SpotReadOutcome.Read:
                        reread.Add(spot);
                        break;
                    case SpotReadOutcome.Unavailable:
                        unavailable.Add(spot.Id);
                        break;
                    case SpotReadOutcome.Failed:
                        break;
                }
            }
        );

        var now = DateTimeOffset.Now.UtcDateTime;
        var rereadSpots = reread.ToList();

        foreach (var spot in rereadSpots)
        {
            spot.ImportedAt = now;
            spot.UpdatedAt = now;
        }

        await dbContext.SaveSpotsAsync(
            _logger,
            async (db, ct) =>
            {
                if (rereadSpots.Count > 0)
                {
                    await db.ExecuteBulkInsertAsync(rereadSpots, ReimportedSpot, ct);

                    // After the spots are stored: the search vector is derived from their stored text.
                    await db.UpsertFtsSpotsAsync(rereadSpots, replaceExisting: true, ct);
                }

                if (!unavailable.IsEmpty)
                {
                    // Their stored attributes are left alone, only the timestamp is stamped so they
                    // are not reread on every run.
                    var unavailableIds = unavailable.ToList();
                    await db
                        .Spots.Where(s => unavailableIds.Contains(s.Id))
                        .ExecuteUpdateAsync(s => s.SetProperty(x => x.ImportedAt, now), ct);
                }
            },
            cancellationToken
        );
    }

    private static OnConflictOptions<Spot> ReimportedSpot =>
        new()
        {
            Update = (existing, inserted) =>
                new Spot
                {
                    Type = inserted.Type,
                    Title = inserted.Title,
                    ReleaseTitle = inserted.ReleaseTitle,
                    Description = inserted.Description,
                    Tag = inserted.Tag,
                    Url = inserted.Url,
                    Filename = inserted.Filename,
                    Newsgroup = inserted.Newsgroup,
                    Spotter = inserted.Spotter,
                    Bytes = inserted.Bytes,
                    NzbMessageIds = inserted.NzbMessageIds,
                    ImageMessageIds = inserted.ImageMessageIds,
                    SpottedAt = inserted.SpottedAt,
                    ImageTypes = inserted.ImageTypes,
                    ImageFormats = inserted.ImageFormats,
                    ImageSources = inserted.ImageSources,
                    ImageLanguages = inserted.ImageLanguages,
                    ImageGenres = inserted.ImageGenres,
                    AudioTypes = inserted.AudioTypes,
                    AudioFormats = inserted.AudioFormats,
                    AudioSources = inserted.AudioSources,
                    AudioBitrates = inserted.AudioBitrates,
                    AudioGenres = inserted.AudioGenres,
                    GamePlatforms = inserted.GamePlatforms,
                    GameFormats = inserted.GameFormats,
                    GameGenres = inserted.GameGenres,
                    GameTypes = inserted.GameTypes,
                    ApplicationPlatforms = inserted.ApplicationPlatforms,
                    ApplicationGenres = inserted.ApplicationGenres,
                    ApplicationTypes = inserted.ApplicationTypes,
                    NewznabCategories = inserted.NewznabCategories,
                    Years = inserted.Years,
                    Seasons = inserted.Seasons,
                    Episodes = inserted.Episodes,
                    ImdbId = inserted.ImdbId,
                    TvdbId = inserted.TvdbId,
                    IndexedAt = inserted.IndexedAt,
                    ImportedAt = inserted.ImportedAt,
                    UpdatedAt = inserted.UpdatedAt,
                },
        };
}
