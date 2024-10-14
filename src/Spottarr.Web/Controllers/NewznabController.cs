using Microsoft.AspNetCore.Mvc;
using Spottarr.Web.Modals.Newznab;

namespace Spottarr.Web.Controllers;

[ApiController]
[Route("[controller]")]
public sealed class NewznabController : ControllerBase
{
    public const string Name = "newznab";
    public const string ActionParameter = "t";

    [HttpGet("caps")]
    public Capabilities Capabilities()
    {
        return new Capabilities
        {
            Server = new ServerInfo
            {
                Name = "test"
            }
        };
    }

    [HttpGet("search")]
    public ActionResult Search()
    {
        return Ok("search");
    }
}