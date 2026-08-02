using Microsoft.AspNetCore.OpenApi;
using Microsoft.OpenApi;
using Spottarr.Web.Auth;

namespace Spottarr.Web.Helpers;

internal sealed class AdminSecuritySchemeTransformer : IOpenApiDocumentTransformer
{
    public const string SchemeId = "AdminApiKey";

    public Task TransformAsync(
        OpenApiDocument document,
        OpenApiDocumentTransformerContext context,
        CancellationToken cancellationToken
    )
    {
        ArgumentNullException.ThrowIfNull(document);

        document.Components ??= new OpenApiComponents();
        document.Components.SecuritySchemes ??= new Dictionary<string, IOpenApiSecurityScheme>();
        document.Components.SecuritySchemes[SchemeId] = new OpenApiSecurityScheme
        {
            Type = SecuritySchemeType.ApiKey,
            Name = AdminAuthenticationHandler.ApiKeyHeaderName,
            In = ParameterLocation.Header,
            Description = "The key configured through Admin.ApiKey.",
        };

        return Task.CompletedTask;
    }
}
