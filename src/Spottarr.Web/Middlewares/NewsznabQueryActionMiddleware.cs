using Spottarr.Web.Controllers;

namespace Spottarr.Web.Middlewares;

/// <summary>
/// Newznab uses the ?t=action query string to determine the action.
/// This middleware converts it to path based routing
/// </summary>
internal sealed class NewsznabQueryActionMiddleware
{
    private readonly RequestDelegate _next;

    public NewsznabQueryActionMiddleware(RequestDelegate next)
    {
        _next = next;
    }
    
    public async Task InvokeAsync(HttpContext context)
    {
        ArgumentNullException.ThrowIfNull(context);
        
        var path = context.Request.Path;
    
        if (!path.StartsWithSegments($"/{NewznabController.Name}", StringComparison.OrdinalIgnoreCase))
            await _next.Invoke(context);
    
        var query = new WriteableQueryCollection(context.Request.Query);
    
        if (!query.TryGetValue(NewznabController.ActionParameter, out var action))
            await _next.Invoke(context);

        query.Remove(NewznabController.ActionParameter);
        context.Request.Query = query;
        context.Request.Path = path.Add($"/{action}");

        await _next.Invoke(context);
    }
}