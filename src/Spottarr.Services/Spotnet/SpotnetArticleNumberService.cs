using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spottarr.Configuration.Options;
using Spottarr.Services.Contracts;
using Spottarr.Services.Logging;
using Spottarr.Services.Parsers;
using Usenet.Exceptions;
using Usenet.Nntp;
using Usenet.Nntp.Contracts;
using Usenet.Nntp.Models;

namespace Spottarr.Services.Spotnet;

internal sealed class SpotnetArticleNumberService : ISpotnetArticleNumberService
{
    private readonly INntpClientPool _nntpClientPool;
    private readonly IOptions<SpotnetOptions> _options;
    private readonly ILogger<SpotnetArticleNumberService> _logger;

    public SpotnetArticleNumberService(INntpClientPool nntpClientPool, IOptions<SpotnetOptions> options,
        ILogger<SpotnetArticleNumberService> logger)
    {
        _nntpClientPool = nntpClientPool;
        _options = options;
        _logger = logger;
    }

    /// <summary>
    /// NNTP does not offer a way to get the closest article number for a specific date.
    /// This makes performing a full import starting at a specific date challenging.
    /// Instead of fetching all headers, this method works around that limitation by performing a binary search.
    /// </summary>
    /// <example>
    /// For range 0 - 10000, check date for 5000, if the date of message 5000 is after the given date,
    /// the given date it must fall in the range of range 0 - 5000.
    /// For range 0 - 5000, check date for 2500, if the date of message 2500 is before the given date,
    /// the given date it must fall in the range of range 2500 - 5000.
    /// For range 2500 - 5000, check date for 3750, if the date of message 3750 is after the given date,
    /// the given date it must fall in the range of range 2500 - 3759.
    /// etc.
    /// </example>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    public async Task<long> GetArticleNumberByDate(CancellationToken cancellationToken)
    {
        var articleNumber = 0L;
        var attempts = 0;
        DateTimeOffset? date = null;

        try
        {
            var options = _options.Value;

            using var lease = await _nntpClientPool.GetLease(cancellationToken);

            // Group is set for the lifetime of the connection
            var groupResponse = await lease.Client.GroupAsync(options.SpotGroup, cancellationToken);
            if (!groupResponse.Success)
            {
                _logger.CouldNotRetrieveSpotGroup(options.SpotGroup, groupResponse.Code, groupResponse.Message);
                return articleNumber;
            }

            var group = groupResponse.Group;

            // Perform the binary search
            var low = group.LowWaterMark;
            var high = group.HighWaterMark;

            while (low <= high)
            {
                date = null;
                var mid = (low + high) / 2L;
                var articleToCheck = mid;

                while (date == null)
                {
                    date = await GetArticleDate(lease.Client, articleToCheck, cancellationToken);
                    attempts++;

                    // Sometimes articles will just be unavailable
                    // In this case retry the next closest article
                    if (date == null) articleToCheck--;

                    // If no article is available when hitting the low watermark, give up. 
                    if (articleToCheck < group.LowWaterMark)
                        throw new InvalidOperationException("No articles available");
                }

                if (date < options.RetrieveAfter) low = mid + 1;
                else high = mid - 1;

                articleNumber = low;
            }

            _logger.FoundArticleNumberForRetrieveAfter(articleNumber, date, group.LowWaterMark, group.HighWaterMark,
                options.RetrieveAfter, attempts);
            return articleNumber;
        }
        catch (NntpException exception)
        {
            _logger.CouldNotRetrieveArticle(exception, string.Empty);
            return articleNumber;
        }
        catch (InvalidOperationException exception)
        {
            _logger.CouldNotRetrieveArticle(exception, string.Empty);
            return articleNumber;
        }
    }

    private async Task<DateTimeOffset?> GetArticleDate(IPooledNntpClient client, long mid,
        CancellationToken cancellationToken)
    {
        var dateResponse = await client.XhdrAsync(NntpHeaders.Date, new NntpArticleRange(mid, mid), cancellationToken);

        if (!dateResponse.Success)
        {
            _logger.CouldNotRetrieveDateHeader(mid, dateResponse.Code, dateResponse.Message);
            return null;
        }

        // Xhdr can return headers for multiple articles, but we only need the first one
        // The header is in the format: <article number> <header value>, strip the article number.
        var dateHeader = dateResponse.Lines
            .FirstOrDefault(string.Empty)
            .Replace($"{mid} ", string.Empty, StringComparison.Ordinal);

        var date = HeaderDateParser.Parse(dateHeader);

        if (!date.HasError) return date.Result;

        _logger.CouldNotParseDateHeader(mid, date.Error);
        return null;
    }
}