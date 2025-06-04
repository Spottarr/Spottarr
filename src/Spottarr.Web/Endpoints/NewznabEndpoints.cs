using System.Net.Mime;
using System.ServiceModel.Syndication;
using Microsoft.AspNetCore.Mvc;
using Spottarr.Data.Entities.Enums;
using Spottarr.Services.Contracts;
using Spottarr.Services.Models;
using Spottarr.Web.EndpointResults;
using Spottarr.Web.Helpers;
using Spottarr.Web.Newznab;
using Spottarr.Web.Newznab.Models;

namespace Spottarr.Web.Endpoints;

public static class NewznabEndpoints
{
    public const string PathPrefix = "/newznab/api";
    public const string ActionParameter = "t";
    private const int DefaultPageSize = 100;

    public static void MapNewznab(this IEndpointRouteBuilder app) =>
        app.MapGroup(PathPrefix)
            .WithTags("Newznab")
            .MapNewznab();

    private static void MapNewznabSearch(this RouteGroupBuilder group, string route) =>
        group.MapGet(route, SearchHandler)
            .Produces(StatusCodes.Status200OK, contentType: MediaTypeNames.Application.Xml);

    private static void MapNewznab(this RouteGroupBuilder group)
    {
        group.MapNewznabSearch("/search");
        group.MapNewznabSearch("/tvsearch");
        group.MapNewznabSearch("/movie");
        group.MapNewznabSearch("/music");
        group.MapNewznabSearch("/book");
        group.MapNewznabSearch("/pc");

        group.MapGet("/caps", (IApplicationVersionService versionService, IHostEnvironment env, HttpRequest request) =>
            {
                var xml = CapabilitiesHelper.GetCapabilities(request.GetApiUri().Uri, request.GetLogoUri().Uri,
                    env.ApplicationName, versionService.Version, DefaultPageSize);

                return new XmlResult<Capabilities>(xml);
            })
            .Produces<Capabilities>(contentType: MediaTypeNames.Application.Xml);

        group.MapGet("/get", async ([FromQuery(Name = "guid")] int id, ISpotImportService spotImportService) =>
            {
                var result = await spotImportService.RetrieveNzb(id);
                return result == null
                    ? Results.NotFound()
                    : Results.File(result, "application/x-nzb", $"{id}.nzb");
            })
            .Produces(StatusCodes.Status200OK, contentType: "application/x-nzb")
            .Produces(StatusCodes.Status404NotFound);
    }

    private static Task<IResult> SearchHandler(
        ISpotSearchService spotSearchService,
        IHostEnvironment env,
        IApplicationVersionService versionService,
        HttpRequest request,
        [FromQuery(Name = "limit")] int limit = DefaultPageSize,
        [FromQuery(Name = "imdbid")] string? imdbId = null,
        [FromQuery(Name = "cat")] string? categories = null,
        [FromQuery(Name = "q")] string? query = null,
        [FromQuery(Name = "ep")] int? episode = null,
        [FromQuery(Name = "season")] int? season = null,
        [FromQuery(Name = "year")] int? year = null,
        [FromQuery(Name = "offset")] int offset = 0
    )
    {
        var clampedLimit = Math.Clamp(limit, 0, 100);
        var parsedCategories = categories?
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(s => Enum.TryParse<NewznabCategory>(s, out var c) ? c : 0)
            .Where(c => c != 0)
            .ToHashSet() ?? [];

        var filter = new SpotSearchFilter
        {
            Offset = offset,
            Limit = clampedLimit,
            Query = query,
            Categories = parsedCategories,
            Years = year.HasValue ? [year.Value] : [],
            Episodes = episode.HasValue ? [episode.Value] : [],
            Seasons = season.HasValue ? [season.Value] : [],
            ImdbId = imdbId == null || imdbId.StartsWith("tt", StringComparison.OrdinalIgnoreCase)
                ? imdbId
                : $"tt{imdbId}"
        };

        return ExecuteSearch(filter, spotSearchService, request);
    }

    private static async Task<IResult> ExecuteSearch(SpotSearchFilter filter, ISpotSearchService spotSearchService,
        HttpRequest request)
    {
        var results = await spotSearchService.Search(filter);
        var items = results.Spots.Select(s => s.ToSyndicationItem(request.GetNzbUri(s.Id).Uri)).ToList();

        var feed = new SyndicationFeed("Spottarr", "Spottarr", request.GetApiUri().Uri, items)
            .AddLogo(request.GetLogoUri().Uri)
            .AddNewznabNamespace()
            .AddNewznabResponseInfo(filter.Offset, results.TotalCount);

        return new NewznabRssResult(feed);
    }
}