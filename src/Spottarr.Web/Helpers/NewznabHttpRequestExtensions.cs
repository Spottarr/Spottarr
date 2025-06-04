using Spottarr.Web.Endpoints;

namespace Spottarr.Web.Helpers;

internal static class NewznabHttpRequestExtensions
{
    public static UriBuilder GetApiUri(this HttpRequest request)
    {
        var b = request.GetBaseUri();
        b.Path = NewznabEndpoints.PathPrefix;
        return b;
    }

    public static UriBuilder GetNzbUri(this HttpRequest request, int id)
    {
        var b = request.GetApiUri();
        b.Query = $"?t=get&guid={id}";
        return b;
    }

    public static UriBuilder GetLogoUri(this HttpRequest request)
    {
        var b = request.GetApiUri();
        b.Path = "/logo.png";
        return b;
    }

    public static UriBuilder GetBaseUri(this HttpRequest request) =>
        new(request.Scheme, request.Host.Host, request.Host.Port ?? -1);
}