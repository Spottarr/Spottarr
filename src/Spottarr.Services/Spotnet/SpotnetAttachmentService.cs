using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Spottarr.Data;
using Spottarr.Services.Contracts;
using Spottarr.Services.Logging;
using Spottarr.Services.Models;
using Spottarr.Services.Parsers;
using Usenet.Exceptions;
using Usenet.Nntp.Contracts;
using Usenet.Nntp.Models;

namespace Spottarr.Services.Spotnet;

internal sealed class SpotnetAttachmentService : ISpotnetAttachmentService
{
    private readonly IDbContextFactory<SpottarrDbContext> _dbContextFactory;
    private readonly INntpClientPool _nntpClientPool;
    private readonly ILogger<SpotnetAttachmentService> _logger;

    public SpotnetAttachmentService(
        IDbContextFactory<SpottarrDbContext> dbContextFactory,
        INntpClientPool nntpClientPool,
        ILogger<SpotnetAttachmentService> logger
    )
    {
        _dbContextFactory = dbContextFactory;
        _nntpClientPool = nntpClientPool;
        _logger = logger;
    }

    public async Task<SpotAttachmentResponse?> FetchNzb(
        int spotId,
        CancellationToken cancellationToken
    )
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var spot = await dbContext.Spots.FirstOrDefaultAsync(
            s => s.Id == spotId,
            cancellationToken
        );
        if (spot == null || string.IsNullOrEmpty(spot.NzbMessageId))
            return null;

        try
        {
            using var lease = await _nntpClientPool.GetLease(cancellationToken);
            var nzbMessageId = spot.NzbMessageId;

            // Only the body carries the NZB payload, so a BODY request avoids transferring and
            // parsing the article headers.
            await using var nzbBodyResponse = await lease.Client.BodyAsync(
                new NntpMessageId(nzbMessageId),
                cancellationToken
            );

            if (!nzbBodyResponse.Success)
            {
                _logger.CouldNotRetrieveArticle(
                    spot.MessageId,
                    nzbBodyResponse.Code,
                    nzbBodyResponse.Message
                );
                return null;
            }

            var stream = await NzbArticleParser.Parse(nzbBodyResponse.Body, cancellationToken);

            return new SpotAttachmentResponse { FileName = spot.Title, Stream = stream };
        }
        catch (NntpException ex)
        {
            _logger.CouldNotRetrieveArticle(ex, spot.MessageId);
            return null;
        }
    }

    public Task<SpotAttachmentResponse?> FetchImage(int spotId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
