using Microsoft.AspNetCore.Mvc;
using Spottarr.Data.Entities.Enums;
using Spottarr.Services.Contracts;
using Spottarr.Services.Models;
using Spottarr.Web.Models;

namespace Spottarr.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class SpotsController : Controller
{
    private readonly ISpotSearchService _spotSearchService;

    public SpotsController(ISpotSearchService spotSearchService) => _spotSearchService = spotSearchService;

    [HttpGet]
    public async Task<IActionResult> Index(
        string? query,
        int page = 0,
        int? episode = null,
        int? season = null,
        int? year = null,
        ISet<NewznabCategory>? newznabCategories = null
    )
    {
        var results = await _spotSearchService.Search(new SpotSearchFilter
        {
            Offset = page,
            Limit = 100,
            Query = query,
            Categories = newznabCategories?.ToHashSet(),
            Years = year.HasValue ? [year.Value] : null,
            Episodes = episode.HasValue ? [episode.Value] : null,
            Seasons = season.HasValue ? [season.Value] : null,
        });

        return Json(results.Spots.Select(s => new SpotTableRowResponseDto(s)));
    }
}