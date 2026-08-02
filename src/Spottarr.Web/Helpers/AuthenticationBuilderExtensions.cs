using Microsoft.AspNetCore.Authentication;
using Spottarr.Web.Auth;

namespace Spottarr.Web.Helpers;

internal static class AuthenticationBuilderExtensions
{
    public static AuthenticationBuilder AddNewznab(this AuthenticationBuilder builder) =>
        builder.AddScheme<AuthenticationSchemeOptions, NewznabAuthenticationHandler>(
            "newznab",
            null
        );

    public static AuthenticationBuilder AddAdmin(this AuthenticationBuilder builder) =>
        builder.AddScheme<AuthenticationSchemeOptions, AdminAuthenticationHandler>(
            AdminAuthenticationHandler.SchemeName,
            null
        );
}
