using System.Net.Mime;
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
    private readonly ISpotImportService _spotImportService;

    public SpotsController(ISpotSearchService spotSearchService, ISpotImportService spotImportService)
    {
        _spotSearchService = spotSearchService;
        _spotImportService = spotImportService;
    }

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
            Categories = newznabCategories?.ToHashSet() ?? [],
            Years = year.HasValue ? [year.Value] : [],
            Episodes = episode.HasValue ? [episode.Value] : [],
            Seasons = season.HasValue ? [season.Value] : [],
        });

        return Json(results.Spots.Select(s => new SpotResponseDto(s)));
    }

    [HttpGet("{id:int}/nzb")]
    [Produces(MediaTypeNames.Text.Xml)]
    public async Task<ActionResult> Nzb(int id)
    {
        var result = await _spotImportService.RetrieveNzb(id);
        if (result == null) return NotFound();

        return File(result, "application/x-nzb", $"{id}.nzb");
    }
}