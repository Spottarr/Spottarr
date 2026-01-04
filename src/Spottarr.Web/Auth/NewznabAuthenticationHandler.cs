using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Spottarr.Configuration.Options;
using Spottarr.Web.Helpers;
using Spottarr.Web.Newznab.Models;

namespace Spottarr.Web.Auth;

internal sealed class NewznabAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    private const string ApiKeyQueryParameterName = "apikey";

    private readonly IOptions<NewznabOptions> _newznabOptions;

    public NewznabAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
        IOptions<NewznabOptions> newznabOptions, ILoggerFactory logger, UrlEncoder encoder)
        : base(options, logger, encoder)
    {
        _newznabOptions = newznabOptions;
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var keyName = ApiKeyQueryParameterName;
        var expectedKey = _newznabOptions.Value.ApiKey;

        var identity = new ClaimsIdentity([
            new(ClaimTypes.NameIdentifier, Scheme.Name),
            new(ClaimTypes.Name, Scheme.Name)
        ], Scheme.Name);

        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);

        // No API key configured, allow all requests
        if (string.IsNullOrEmpty(expectedKey))
            return Task.FromResult(AuthenticateResult.Success(ticket));

        if (!Request.Query.TryGetValue(keyName, out var providedKey) || StringValues.IsNullOrEmpty(providedKey))
            return Task.FromResult(AuthenticateResult.Fail("An API key was configured, but none was provided."));

        if (!string.Equals(providedKey!, expectedKey, StringComparison.Ordinal))
            return Task.FromResult(AuthenticateResult.Fail("Invalid API key"));

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }

    protected override Task HandleChallengeAsync(AuthenticationProperties properties) => WriteErrorResponse(Response);
    protected override Task HandleForbiddenAsync(AuthenticationProperties properties) => WriteErrorResponse(Response);

    private static Task WriteErrorResponse(HttpResponse response)
    {
        // https://newznab.readthedocs.io/en/latest/misc/api.html#newznab-error-codes
        // Newznab expects a 200 OK with an error XML for incorrect credentials
        response.StatusCode = StatusCodes.Status200OK;

        var error = new Error
        {
            Code = ErrorCode.IncorrectUserCredentials,
            Description = "Incorrect user credentials"
        };

        return response.WriteAsXmlAsync(error, "error", CancellationToken.None);
    }
}