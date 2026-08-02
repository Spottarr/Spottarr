using Quartz;
using Spottarr.Services.Contracts;
using Spottarr.Services.Jobs;
using Spottarr.Services.Models;
using Spottarr.Web.Api;
using Spottarr.Web.Api.Models;
using Spottarr.Web.Auth;

namespace Spottarr.Web.Endpoints;

internal static class ApiEndpoints
{
    public const string PathPrefix = "/api/v1";

    public static void MapApi(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup(PathPrefix)
            .WithTags("Spottarr")
            .RequireAuthorization(AdminAuthenticationHandler.SchemeName);

        group
            .MapGet(
                "/spots/{id:int}",
                async (
                    int id,
                    ISpotSearchService spotSearchService,
                    CancellationToken cancellationToken
                ) =>
                {
                    var results = await spotSearchService.Search(
                        new SpotSearchFilter { Id = id, Limit = 1 },
                        cancellationToken
                    );

                    var spot = results.Spots.FirstOrDefault();

                    return spot == null ? Results.NotFound() : Results.Ok(spot.ToResponse());
                }
            )
            .Produces<SpotResponse>()
            .Produces(StatusCodes.Status404NotFound)
            .WithDescription(
                "Get a single spot, including whether it is flagged for reprocessing."
            );

        group.MapSpotFlag(
            "/spots/reimport",
            SpotOperation.Reimport,
            "reread from usenet and overwritten in place"
        );
        group.MapSpotFlag(
            "/spots/reindex",
            SpotOperation.Reindex,
            "have their indexable attributes derived again"
        );
    }

    private static void MapSpotFlag(
        this RouteGroupBuilder group,
        string route,
        SpotOperation operation,
        string description
    )
    {
        group
            .MapGet(
                route,
                async (
                    ISpotFlagService spotFlagService,
                    ISchedulerFactory schedulerFactory,
                    CancellationToken cancellationToken
                ) =>
                    new SpotFlagStatusResponse
                    {
                        Flagged = await spotFlagService.CountFlagged(operation, cancellationToken),
                        Running = await IsImportRunning(schedulerFactory, cancellationToken),
                    }
            )
            .WithDescription($"Count the spots that are flagged to be {description}.");

        group
            .MapPost(
                route,
                async (
                    SpotSelectionRequest request,
                    ISpotFlagService spotFlagService,
                    ISchedulerFactory schedulerFactory,
                    CancellationToken cancellationToken
                ) =>
                {
                    if (!request.TryCreateSelection(out var selection, out var error))
                        return Results.BadRequest(error);

                    var flagged = await spotFlagService.Flag(
                        operation,
                        selection,
                        cancellationToken
                    );

                    // The flags survive on their own, triggering only avoids waiting for the schedule.
                    var scheduler = await schedulerFactory.GetScheduler(cancellationToken);
                    await scheduler.TriggerJob(JobKeys.ImportSpots, cancellationToken);

                    return Results.Accepted(value: new SpotFlaggedResponse { Flagged = flagged });
                }
            )
            .Produces<SpotFlaggedResponse>(StatusCodes.Status202Accepted)
            .Produces(StatusCodes.Status400BadRequest)
            .WithDescription($"Flag the selected spots to be {description}.");

        group
            .MapDelete(
                route,
                async (ISpotFlagService spotFlagService, CancellationToken cancellationToken) =>
                    new SpotFlagsClearedResponse
                    {
                        Cleared = await spotFlagService.ClearFlags(operation, cancellationToken),
                    }
            )
            .WithDescription(
                $"Clear the flags of all spots that are waiting to be {description}. "
                    + "Spots that are already being processed are finished."
            );
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
