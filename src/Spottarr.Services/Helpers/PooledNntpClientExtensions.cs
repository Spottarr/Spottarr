using Spottarr.Configuration.Options;
using Usenet.Nntp.Contracts;
using Usenet.Nntp.Models;
using Usenet.Nntp.Responses;

namespace Spottarr.Services.Helpers;

internal static class PooledNntpClientExtensions
{
    public static Task<NntpStreamResponse<NntpHeaderField>> GetHeadersAsync(
        this IPooledNntpClient client,
        UsenetOptions options,
        string field,
        NntpArticleRange range,
        CancellationToken cancellationToken
    ) =>
        options.Compression == UsenetCompression.OperationExtensions
            ? client.XzhdrAsync(field, range, cancellationToken)
            : client.XhdrAsync(field, range, cancellationToken);

    public static Task<NntpStreamResponse<NntpArticleOverview>> GetOverviewAsync(
        this IPooledNntpClient client,
        UsenetOptions options,
        NntpArticleRange range,
        CancellationToken cancellationToken
    ) =>
        options.Compression == UsenetCompression.OperationExtensions
            ? client.XzverAsync(range, cancellationToken)
            : client.XoverAsync(range, cancellationToken);
}
