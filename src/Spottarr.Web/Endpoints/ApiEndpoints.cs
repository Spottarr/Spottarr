using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.OpenApi;
using Quartz;
using Spottarr.Services.Contracts;
using Spottarr.Services.Jobs;
using Spottarr.Services.Models;
using Spottarr.Web.Api;
using Spottarr.Web.Api.Models;
using Spottarr.Web.Auth;
using Spottarr.Web.Helpers;

namespace Spottarr.Web.Endpoints;

internal static class ApiEndpoints
{
    public const string PathPrefix = "/api/v1";

    public static void MapApi(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(PathPrefix)
            .WithTags("Spottarr")
            .RequireAuthorization(AdminAuthenticationHandler.SchemeName)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .AddOpenApiOperationTransformer(
                (operation, context, cancellationToken) =>
                {
                    operation.Security =
                    [
                        new OpenApiSecurityRequirement
                        {
                            [
                                new OpenApiSecuritySchemeReference(
                                    AdminSecuritySchemeTransformer.SchemeId,
                                    context.Document
                                )
                            ] = [],
                        },
                    ];

                    return Task.CompletedTask;
                }
            );

        group
            .MapGet("/spots/{id:int}", GetSpot)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .WithDescription(
                "Get a single spot, including whether it is waiting to be reprocessed."
            );

        group
            .MapGet("/spots/reimport", CountMarkedForReimport)
            .WithDescription("Count the spots that are waiting to be reread from usenet.");
        group
            .MapPost("/spots/reimport", MarkForReimport)
            .WithDescription("Mark the selected spots to be reread from usenet.");
        group
            .MapDelete("/spots/reimport", UnmarkForReimport)
            .WithDescription(
                "Unmark the spots that are waiting to be reread from usenet. "
                    + "Spots that are being reread at that moment are finished."
            );

        group
            .MapGet("/spots/reindex", CountMarkedForReindex)
            .WithDescription("Count the spots that are waiting to be indexed again.");
        group
            .MapPost("/spots/reindex", MarkForReindex)
            .WithDescription(
                "Mark the selected spots to have their indexable attributes derived again."
            );
        group
            .MapDelete("/spots/reindex", UnmarkForReindex)
            .WithDescription(
                "Unmark the spots that are waiting to be indexed again. "
                    + "Spots that are being indexed at that moment are finished."
            );
    }

    private static async Task<Results<Ok<SpotResponse>, NotFound>> GetSpot(
        int id,
        ISpotSearchService spotSearchService,
        CancellationToken cancellationToken
    )
    {
        var results = await spotSearchService.Search(
            new SpotSearchFilter { Id = id, Limit = 1 },
            cancellationToken
        );

        var spot = results.Spots.FirstOrDefault();

        return spot == null ? TypedResults.NotFound() : TypedResults.Ok(spot.ToResponse());
    }

    private static async Task<Ok<MarkedSpotsStatusResponse>> CountMarkedForReimport(
        ISpotReimportService spotReimportService,
        ISchedulerFactory schedulerFactory,
        CancellationToken cancellationToken
    ) =>
        TypedResults.Ok(
            new MarkedSpotsStatusResponse
            {
                Marked = await spotReimportService.CountMarkedForReimport(cancellationToken),
                Running = await IsImportRunning(schedulerFactory, cancellationToken),
            }
        );

    private static async Task<
        Results<Accepted<MarkedSpotsResponse>, ValidationProblem>
    > MarkForReimport(
        SpotSelectionRequest request,
        ISpotReimportService spotReimportService,
        ISchedulerFactory schedulerFactory,
        CancellationToken cancellationToken
    )
    {
        if (!request.TryCreateSelection(out var selection, out var errors))
            return TypedResults.ValidationProblem(errors);

        var marked = await spotReimportService.MarkForReimport(selection, cancellationToken);
        await TriggerImport(schedulerFactory, marked, cancellationToken);

        return TypedResults.Accepted((string?)null, new MarkedSpotsResponse { Marked = marked });
    }

    private static async Task<Ok<UnmarkedSpotsResponse>> UnmarkForReimport(
        ISpotReimportService spotReimportService,
        CancellationToken cancellationToken
    ) =>
        TypedResults.Ok(
            new UnmarkedSpotsResponse
            {
                Unmarked = await spotReimportService.UnmarkForReimport(cancellationToken),
            }
        );

    private static async Task<Ok<MarkedSpotsStatusResponse>> CountMarkedForReindex(
        ISpotReindexService spotReindexService,
        ISchedulerFactory schedulerFactory,
        CancellationToken cancellationToken
    ) =>
        TypedResults.Ok(
            new MarkedSpotsStatusResponse
            {
                Marked = await spotReindexService.CountMarkedForReindex(cancellationToken),
                Running = await IsImportRunning(schedulerFactory, cancellationToken),
            }
        );

    private static async Task<
        Results<Accepted<MarkedSpotsResponse>, ValidationProblem>
    > MarkForReindex(
        SpotSelectionRequest request,
        ISpotReindexService spotReindexService,
        ISchedulerFactory schedulerFactory,
        CancellationToken cancellationToken
    )
    {
        if (!request.TryCreateSelection(out var selection, out var errors))
            return TypedResults.ValidationProblem(errors);

        var marked = await spotReindexService.MarkForReindex(selection, cancellationToken);
        await TriggerImport(schedulerFactory, marked, cancellationToken);

        return TypedResults.Accepted((string?)null, new MarkedSpotsResponse { Marked = marked });
    }

    private static async Task<Ok<UnmarkedSpotsResponse>> UnmarkForReindex(
        ISpotReindexService spotReindexService,
        CancellationToken cancellationToken
    ) =>
        TypedResults.Ok(
            new UnmarkedSpotsResponse
            {
                Unmarked = await spotReindexService.UnmarkForReindex(cancellationToken),
            }
        );

    /// <summary>
    /// The marked spots are processed by the scheduled import job, triggering it only avoids waiting
    /// for the next scheduled run.
    /// </summary>
    private static async Task TriggerImport(
        ISchedulerFactory schedulerFactory,
        int marked,
        CancellationToken cancellationToken
    )
    {
        if (marked == 0)
            return;

        var scheduler = await schedulerFactory.GetScheduler(cancellationToken);
        await scheduler.TriggerJob(JobKeys.ImportSpots, cancellationToken);
    }

    private static async Task<bool> IsImportRunning(
        ISchedulerFactory schedulerFactory,
        CancellationToken cancellationToken
    )
    {
        var scheduler = await schedulerFactory.GetScheduler(cancellationToken);
        var running = await scheduler.GetCurrentlyExecutingJobs(cancellationToken);

        return running.Any(j => j.JobDetail.Key.Equals(JobKeys.ImportSpots));
    }
}
