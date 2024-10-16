using System.ServiceModel.Syndication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Spottarr.Data;
using Spottarr.Services.Contracts;
using Spottarr.Web.Helpers;
using Spottarr.Web.Modals.Newznab;

namespace Spottarr.Web.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class NewznabController : ControllerBase
{
    public const string Name = "newznab";
    public const string ActionParameter = "t";
    
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
    [Produces("application/rss+xml")]
    public async Task<ActionResult> Search(bool dl = true)
    {
        var uriBuilder = new UriBuilder(Request.Scheme, Request.Host.Host, Request.Host.Port ?? -1);

        var spots = await _dbContext.Spots.Take(100).ToListAsync();
        
        var items = spots.Select(s => new SyndicationItem(s.Subject, s.Subject, uriBuilder.Uri, s.MessageId, s.UpdatedAt)
            .AddNewznabAttribute("category", "5000")
            .AddNewznabAttribute("subs", "dutch,english")).ToList();

        var feed = new SyndicationFeed("Spottarr Index", "Spottarr Index API", uriBuilder.Uri, items)
            .AddNewznabNamespace();

        return File(NewznabRssSerializer.Serialize(feed), dl ? "application/rss+xml": "text/xml");
    }
}