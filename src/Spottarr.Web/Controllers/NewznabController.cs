using System.Net.Mime;
using System.ServiceModel.Syndication;
using Microsoft.AspNetCore.Mvc;
using Spottarr.Data.Entities.Enums;
using Spottarr.Services.Contracts;
using Spottarr.Services.Models;
using Spottarr.Web.Helpers;
using Spottarr.Web.Newznab;
using Spottarr.Web.Newznab.Models;

namespace Spottarr.Web.Controllers;

[ApiController]
[Route("[controller]/api")]
public sealed class NewznabController : Controller
{
    public const string Name = "newznab";
    public const string ActionParameter = "t";
    private const int DefaultPageSize = 100;

    private readonly IApplicationVersionService _applicationVersionService;
    private readonly IHostEnvironment _hostEnvironment;
    private readonly ISpotSearchService _spotSearchService;
    private readonly ISpotImportService _spotImportService;

    public NewznabController(IApplicationVersionService applicationVersionService, IHostEnvironment hostEnvironment,
        ISpotSearchService spotSearchService, ISpotImportService spotImportService)
    {
        _applicationVersionService = applicationVersionService;
        _hostEnvironment = hostEnvironment;
        _spotSearchService = spotSearchService;
        _spotImportService = spotImportService;
    }

    [HttpGet("caps")]
    [Produces("application/xml")]
    public Capabilities Capabilities()
    {
        var uriBuilder = new UriBuilder(Request.Scheme, Request.Host.Host, Request.Host.Port ?? -1);
        uriBuilder.Path = "/newznab/api";
        var uri = uriBuilder.ToString();
        uriBuilder.Path = "/logo.png";
        var imageUri = uriBuilder.ToString();

        // https://github.com/Prowlarr/Prowlarr/blob/develop/src/NzbDrone.Core/Indexers/IndexerCapabilities.cs
        return new Capabilities
        {
            ServerInfo = new ServerInfo
            {
                Title = _hostEnvironment.ApplicationName,
                Version = _applicationVersionService.Version,
                Tagline = _hostEnvironment.ApplicationName,
                Email = string.Empty,
                Host = uri,
                Image = imageUri,
                Type = _hostEnvironment.ApplicationName,
            },
            Limits = new Limits
            {
                Max = DefaultPageSize,
                Default = DefaultPageSize,
            },
            Registration = new Registration
            {
                Available = "no",
                Open = "no"
            },
            Searching = new Searching
            {
                Search = new Search
                {
                    Available = "yes",
                    SupportedParams = "q",
                },
                TvSearch = new Search
                {
                    Available = "yes",
                    SupportedParams = "q,season,ep,year",
                },
                MovieSearch = new Search
                {
                    Available = "yes",
                    SupportedParams = "q,season,ep,year",
                },
                AudioSearch = new Search
                {
                    Available = "yes",
                    SupportedParams = "q,year",
                },
                PcSearch = new Search()
                {
                    Available = "no",
                    SupportedParams = "",
                },
                BookSearch = new Search
                {
                    Available = "yes",
                    SupportedParams = "q,title",
                }
            },
            Categories = []
        };
    }

    [HttpGet("search")]
    [HttpGet("tvsearch")]
    [HttpGet("movie")]
    [HttpGet("music")]
    [HttpGet("book")]
    [HttpGet("pc")]
    [Produces(MediaTypeNames.Text.Xml)]
    public async Task<ActionResult> Search(
        string? q,
        int limit = DefaultPageSize,
        int offset = 0,
        int? ep = null,
        int? season = null,
        int? year = null,
        [FromQuery, ModelBinder<CommaSeparatedEnumBinder>] NewznabCategory[]? cat = null
    )
    {
        var uriBuilder = new UriBuilder(Request.Scheme, Request.Host.Host, Request.Host.Port ?? -1);
        uriBuilder.Path = "/newznab/api";
        var apiUri = uriBuilder.Uri;
        var spotUriTemplate = $"{uriBuilder}?t=get&guid={{0}}";
        
        var clampedLimit = Math.Clamp(limit, 0, DefaultPageSize);
        var results = await _spotSearchService.Search(new SpotSearchFilter()
        {
            Offset = offset,
            Limit = clampedLimit,
            Query = q,
            Categories = cat?.ToHashSet(),
            Years = year.HasValue ? [year.Value] : null,
            Episodes = ep.HasValue ? [ep.Value] : null,
            Seasons = season.HasValue ? [season.Value] : null,
        });

        var items = results.Spots.Select(s => s.ToSyndicationItem(spotUriTemplate)).ToList();

        var feed = new SyndicationFeed(_hostEnvironment.ApplicationName, _hostEnvironment.ApplicationName, apiUri, items)
            .AddNewznabNamespace()
            .AddNewznabResponseInfo(offset, results.TotalCount);

        return File(NewznabRssSerializer.Serialize(feed), MediaTypeNames.Text.Xml);
    }
}