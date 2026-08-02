using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Spottarr.Configuration.Options;

namespace Spottarr.Web.Auth;

internal sealed class AdminAuthenticationHandler
    : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public const string SchemeName = "admin";
    public const string ApiKeyHeaderName = "X-Api-Key";

    private readonly IOptions<AdminOptions> _adminOptions;

    public AdminAuthenticationHandler(
        IOptionsMonitor<AuthenticationSchemeOptions> options,
        IOptions<AdminOptions> adminOptions,
        ILoggerFactory logger,
        UrlEncoder encoder
    )
        : base(options, logger, encoder)
    {
        _adminOptions = adminOptions;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var expectedKey = _adminOptions.Value.ApiKey;

        // The administrative API can change or delete indexed spots, so it stays closed until a key
        // is configured, unlike the read only newznab API.
        if (string.IsNullOrEmpty(expectedKey))
            return Task.FromResult(
                AuthenticateResult.Fail(
                    "No administrative API key is configured. Set Admin.ApiKey to enable this API."
                )
            );

        if (
            !Request.Headers.TryGetValue(ApiKeyHeaderName, out var providedKey)
            || StringValues.IsNullOrEmpty(providedKey)
        )
            return Task.FromResult(
                AuthenticateResult.Fail(
                    $"No API key was provided in the {ApiKeyHeaderName} header."
                )
            );

        // Compare in constant time so the response timing does not leak how much of the key matched.
        var providedKeyBytes = Encoding.UTF8.GetBytes(providedKey.ToString());
        var expectedKeyBytes = Encoding.UTF8.GetBytes(expectedKey);
        if (!CryptographicOperations.FixedTimeEquals(providedKeyBytes, expectedKeyBytes))
            return Task.FromResult(AuthenticateResult.Fail("Invalid API key"));

        var identity = new ClaimsIdentity(
            [new(ClaimTypes.NameIdentifier, Scheme.Name), new(ClaimTypes.Name, Scheme.Name)],
            Scheme.Name
        );

        return Task.FromResult(
            AuthenticateResult.Success(
                new AuthenticationTicket(new ClaimsPrincipal(identity), Scheme.Name)
            )
        );
    }
}
