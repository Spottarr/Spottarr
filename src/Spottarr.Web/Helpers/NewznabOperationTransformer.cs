using System.Text.RegularExpressions;
using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi.Models;
using Spottarr.Web.Controllers;

namespace Spottarr.Web.Helpers;

internal sealed partial class NewznabOperationTransformer : IOpenApiDocumentTransformer
{
    [GeneratedRegex($"^({NewznabController.PathPrefix})/(?<action>[^/]+)", RegexOptions.IgnoreCase)]
    private static partial Regex NewznabPathRegex();

    public Task TransformAsync(OpenApiDocument document, OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken)
    {
        var regex = NewznabPathRegex();
        var adjustedPaths = new OpenApiPaths();
        foreach (var (key, value) in document.Paths)
        {
            var match = regex.Match(key);
            var newKey = match.Success ? $"{match.Groups[1].Value}?t={match.Groups[2].Value}" : key;
            adjustedPaths.Add(newKey, value);
        }

        document.Paths = adjustedPaths;

        return Task.CompletedTask;
    }
}