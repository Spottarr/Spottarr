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

internal static class NewznabEndpoints
{
    public const string PathPrefix = "/newznab/api";
    public const string ActionParameter = "t";
    private const int DefaultPageSize = 100;

    public static void MapNewznab(this IEndpointRouteBuilder app) =>
        app.MapGroup(PathPrefix)
            .WithTags("Newznab")
            .MapNewznab();

    private static RouteHandlerBuilder MapNewznabSearch(this RouteGroupBuilder group, string route) =>
        group.MapGet(route, SearchHandler)
            .Produces(StatusCodes.Status200OK, contentType: MediaTypeNames.Application.Xml)
            .RequireAuthorization("newznab");

    private static void MapNewznab(this RouteGroupBuilder group)
    {
        group.MapNewznabSearch("/search")
            .WithDescription("Search for any type of spot. Returns a Newznab compatible RSS feed.");
        group.MapNewznabSearch("/tvsearch")
            .WithDescription("Search for TV show spots. Returns a Newznab compatible RSS feed.");
        group.MapNewznabSearch("/movie")
            .WithDescription("Search for movie spots. Returns a Newznab compatible RSS feed.");
        group.MapNewznabSearch("/music")
            .WithDescription("Search for music spots. Returns a Newznab compatible RSS feed.");
        group.MapNewznabSearch("/book")
            .WithDescription("Search for book spots. Returns a Newznab compatible RSS feed.");
        group.MapNewznabSearch("/pc")
            .WithDescription("Search for software spots. Returns a Newznab compatible RSS feed.");

        group.MapGet("/caps", (IApplicationVersionService versionService, IHostEnvironment env, HttpRequest request) =>
            {
                var xml = CapabilitiesHelper.GetCapabilities(request.GetApiUri().Uri, request.GetLogoUri().Uri,
                    env.ApplicationName, versionService.Version, DefaultPageSize);

                return new XmlResult<Capabilities>("caps", xml);
            })
            .Produces<Capabilities>(contentType: MediaTypeNames.Application.Xml)
            .WithDescription("Get the Newznab capabilities that Spottarr supports.");

        group.MapGet("/get",
                async ([FromQuery(Name = "guid")] int id, ISpotnetAttachmentService spotnetAttachmentService,
                    CancellationToken cancellationToken) =>
                {
                    var result = await spotnetAttachmentService.FetchNzb(id, cancellationToken);
                    return result == null
                        ? Results.NotFound()
                        : Results.File(result.Stream, "application/x-nzb", $"{result.FileName}.nzb");
                })
            .Produces(StatusCodes.Status200OK, contentType: "application/x-nzb")
            .Produces(StatusCodes.Status404NotFound)
            .WithDescription("Get the NZB file for a spot.");
    }

    private static Task<IResult> SearchHandler(
        ISpotSearchService spotSearchService,
        IHostEnvironment env,
        IApplicationVersionService versionService,
        HttpRequest request,
        CancellationToken cancellationToken,
        [FromQuery(Name = "guid")] int id = 0,
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
            Id = id,
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

        return ExecuteSearch(filter, spotSearchService, request, cancellationToken);
    }

    private static async Task<IResult> ExecuteSearch(SpotSearchFilter filter, ISpotSearchService spotSearchService,
        HttpRequest request, CancellationToken cancellationToken)
    {
        var results = await spotSearchService.Search(filter, cancellationToken);
        var items = results.Spots
            .Select(s => s.ToSyndicationItem(request.GetDetailsUri(s.Id).Uri, request.GetNzbUri(s.Id).Uri)).ToList();

        var feed = new SyndicationFeed("Spottarr", "Spottarr", request.GetApiUri().Uri, items)
            .AddLogo(request.GetLogoUri().Uri)
            .AddNewznabNamespace()
            .AddNewznabResponseInfo(filter.Offset, results.TotalCount);

        return new NewznabRssResult(feed);
    }
}