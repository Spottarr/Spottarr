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
    public Capabilities Capabilities() =>
        CapabilitiesHelper.GetCapabilities(GetApiUri().Uri, GetLogoUri().Uri, _hostEnvironment.ApplicationName,
            _applicationVersionService.Version, DefaultPageSize);

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
        [FromQuery, ModelBinder<CommaSeparatedEnumBinder>]
        NewznabCategory[]? cat = null
    )
    {
        var clampedLimit = Math.Clamp(limit, 0, DefaultPageSize);
        var results = await _spotSearchService.Search(new SpotSearchFilter
        {
            Offset = offset,
            Limit = clampedLimit,
            Query = q,
            Categories = cat?.ToHashSet(),
            Years = year.HasValue ? [year.Value] : null,
            Episodes = ep.HasValue ? [ep.Value] : null,
            Seasons = season.HasValue ? [season.Value] : null,
        });

        var items = results.Spots.Select(s => s.ToSyndicationItem(GetNzbUri(s.Id).Uri)).ToList();

        var feed = new SyndicationFeed(_hostEnvironment.ApplicationName, _hostEnvironment.ApplicationName,
                GetApiUri().Uri, items)
            .AddLogo(GetLogoUri().Uri)
            .AddNewznabNamespace()
            .AddNewznabResponseInfo(offset, results.TotalCount);

        return File(NewznabRssSerializer.Serialize(feed), MediaTypeNames.Text.Xml);
    }

    [HttpGet("get")]
    [Produces(MediaTypeNames.Text.Xml)]
    public async Task<ActionResult> Get([FromQuery(Name = "guid")] int id)
    {
        var result = await _spotImportService.RetrieveNzb(id);
        if (result == null) return NotFound();

        return File(result, "application/x-nzb", $"{id}.nzb");
    }

    private UriBuilder GetApiUri()
    {
        var b = GetBaseUri();
        b.Path = "/newznab/api";
        return b;
    }

    private UriBuilder GetNzbUri(int id)
    {
        var b = GetApiUri();
        b.Query = $"?t=get&guid={id}";
        return b;
    }

    private UriBuilder GetLogoUri()
    {
        var b = GetApiUri();
        b.Path = "/logo.png";
        return b;
    }

    private UriBuilder GetBaseUri() => new(Request.Scheme, Request.Host.Host, Request.Host.Port ?? -1)
    {
        Path = "/newznab/api",
    };
}