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
        if (spot == null || spot.NzbMessageIds.Count == 0)
            return null;

        try
        {
            using var lease = await _nntpClientPool.GetLease(cancellationToken);

            // The attachment is split over one or more articles and only inflates as a whole, so the
            // bodies are concatenated in order before being decoded. Each body is copied out before its
            // response is disposed, because the response owns the pooled buffer backing it.
            using var payload = new MemoryStream();

            foreach (var nzbMessageId in spot.NzbMessageIds)
            {
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

                payload.Write(nzbBodyResponse.Body.Span);
            }

            var stream = await NzbArticleParser.Parse(
                payload.GetBuffer().AsMemory(0, (int)payload.Length),
                cancellationToken
            );

            return new SpotAttachmentResponse { FileName = spot.Title, Stream = stream };
        }
        catch (NntpException ex)
        {
            _logger.CouldNotRetrieveArticle(ex, spot.MessageId);
            return null;
        }
        catch (InvalidDataException ex)
        {
            _logger.CouldNotDecodeAttachment(ex, spot.MessageId);
            return null;
        }
    }

    public Task<SpotAttachmentResponse?> FetchImage(int spotId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}
