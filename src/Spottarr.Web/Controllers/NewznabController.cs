using System.ServiceModel.Syndication;
using System.Text;
using System.Xml;
using Microsoft.AspNetCore.Mvc;
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
    
    private readonly IHostEnvironment _hostEnvironment;
    private readonly IApplicationVersionService _applicationVersionService;
    
    public NewznabController(IApplicationVersionService applicationVersionService, IHostEnvironment hostEnvironment)
    {
        _hostEnvironment = hostEnvironment;
        _applicationVersionService = applicationVersionService;
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
    public ActionResult Search()
    {
        var uriBuilder = new UriBuilder(Request.Scheme, Request.Host.Host, Request.Host.Port ?? -1);
        var feed = new SyndicationFeed("Spottarr Index", "Spottarr Index API", uriBuilder.Uri)
        {
            Items = new List<SyndicationItem>
            {
                new("test 1", "test 1", uriBuilder.Uri, "1", DateTimeOffset.Now),
                new("test 2", "test 2", uriBuilder.Uri, "2", DateTimeOffset.Now),
            }
        };

        return new RssFeedResult(feed);
    }
}