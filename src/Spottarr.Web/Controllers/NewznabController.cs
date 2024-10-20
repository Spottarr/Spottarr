using System.Net.Mime;
using System.ServiceModel.Syndication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spottarr.Data;
using Spottarr.Data.Entities.Enums;
using Spottarr.Services.Contracts;
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
    private readonly SpottarrDbContext _dbContext;
    
    public NewznabController(IApplicationVersionService applicationVersionService, IHostEnvironment hostEnvironment, SpottarrDbContext dbContext)
    {
        _applicationVersionService = applicationVersionService;
        _hostEnvironment = hostEnvironment;
        _dbContext = dbContext;
    }
    
    [HttpGet("caps")]
    [Produces("application/xml")]
    public Capabilities Capabilities()
    {
        var uriBuilder = new UriBuilder(Request.Scheme, Request.Host.Host, Request.Host.Port ?? -1);
        var uri = uriBuilder.ToString();
        uriBuilder.Path = "/logo.png";
        var imageUri = uriBuilder.ToString();
        
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
                Max = 500,
                Default = 100,
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
                    SupportedParams = "q",
                },
                MovieSearch = new Search
                {
                    Available = "yes",
                    SupportedParams = "q",
                },
                AudioSearch = new Search
                {
                    Available = "yes",
                    SupportedParams = "q",
                },
                PcSearch = new Search
                {
                    Available = "yes",
                    SupportedParams = "q",
                },
                BookSearch = new Search
                {
                    Available = "yes",
                    SupportedParams = "q",
                }
            },
            Categories =
            [
                new()
                {
                    Id = 1,
                    Name = "Test 1",
                    SubCategories =
                    [
                        new()
                        {
                            Id = 11,
                            Name = "Test 1.1"
                        },

                        new()
                        {
                            Id = 12,
                            Name = "Test 1.2"
                        }
                    ]
                },

                new()
                {
                    Id = 2,
                    Name = "Test 2",
                    SubCategories =
                    [
                        new()
                        {
                            Id = 21,
                            Name = "Test 2.1"
                        },

                        new()
                        {
                            Id = 22,
                            Name = "Test 2.2"
                        }
                    ]
                }
            ],
        };
    }

    [HttpGet("search")]
    [HttpGet("tvsearch")]
    [HttpGet("movie")]
    [HttpGet("music")]
    [HttpGet("book")]
    [HttpGet("pc")]
    [Produces(MediaTypeNames.Text.Xml)]
    public async Task<ActionResult> Search(string? q, int limit = DefaultPageSize, int offset = 0,
        [FromQuery,ModelBinder<CommaSeparatedEnumBinder>] NewznabCategory[]? cat = null)
    {
        var uriBuilder = new UriBuilder(Request.Scheme, Request.Host.Host, Request.Host.Port ?? -1);
        
        var categories = cat ?? [];
        var pageSize = Math.Clamp(limit, 0, DefaultPageSize);
        var pageNumber = offset;
        var spotQuery = _dbContext.Spots.AsQueryable();
            
        if(q != null)
            spotQuery = spotQuery.Where(s => s.Title.Contains(q) || (s.Description != null && s.Description.Contains(q)));

        var spots = await spotQuery
            .OrderByDescending(s => s.SpottedAt)
            .Skip(pageNumber * pageSize)
            .Take(pageSize)
            .ToListAsync();
        
        var items = spots.Select(s => s.ToSyndicationItem(uriBuilder.Uri)).ToList();

        var feed = new SyndicationFeed("Spottarr Index", "Spottarr Index API", uriBuilder.Uri, items)
            .AddNewznabNamespace()
            .AddNewznabResponseInfo(pageNumber, pageSize);
        
        return File(NewznabRssSerializer.Serialize(feed), MediaTypeNames.Text.Xml);
    }
}