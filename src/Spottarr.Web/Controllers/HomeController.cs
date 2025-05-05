using Microsoft.AspNetCore.Mvc;
using Spottarr.Services.Contracts;

namespace Spottarr.Web.Controllers;

[Route("")]
public class HomeController : Controller
{
    private readonly ISpotSearchService _spotSearchService;
    private readonly IApplicationVersionService _versionService;

    public HomeController(ISpotSearchService spotSearchService, IApplicationVersionService versionService)
    {
        _spotSearchService = spotSearchService;
        _versionService = versionService;
    }

    public async Task<IActionResult> Index()
    {
        var totalCount = await _spotSearchService.Count();
        return View(new Stats(_versionService.Version, totalCount));
    }
}

public record Stats(string Version, int SpotCount);