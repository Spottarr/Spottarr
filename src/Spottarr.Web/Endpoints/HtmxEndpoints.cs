using Spottarr.Services.Contracts;
using Spottarr.Web.EndpointResults;

namespace Spottarr.Web.Endpoints;

internal static class HtmxEndpoints
{
    public const string PathPrefix = "/htmx";

    public static void MapHtmx(this IEndpointRouteBuilder app) =>
        app.MapGroup(PathPrefix)
            .ExcludeFromDescription()
            .MapStats();

    private static void MapStats(this RouteGroupBuilder group) =>
        group.MapGet("/stats",
            async (ISpotSearchService spotSearchService, IApplicationVersionService versionService) =>
            {
                var totalCount = await spotSearchService.Count();
                var version = versionService.Version.Split('+').FirstOrDefault();

                return new HtmlResult($"""
                                       <p class="stats">Spots indexed: {totalCount}</p>
                                       <p class="stats">Version: {version}</p>
                                       """);
            });
}