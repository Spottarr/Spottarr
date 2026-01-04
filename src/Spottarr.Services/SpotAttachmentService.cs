using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Spottarr.Data;
using Spottarr.Services.Contracts;
using Spottarr.Services.Logging;
using Spottarr.Services.Parsers;
using Usenet.Exceptions;
using Usenet.Nntp.Contracts;
using Usenet.Nntp.Models;

namespace Spottarr.Services;

internal sealed class SpotAttachmentService : ISpotAttachmentService
{
    private readonly IDbContextFactory<SpottarrDbContext> _dbContextFactory;
    private readonly INntpClientPool _nntpClientPool;
    private readonly ILogger<SpotAttachmentService> _logger;

    public SpotAttachmentService(
        IDbContextFactory<SpottarrDbContext> dbContextFactory,
        INntpClientPool nntpClientPool,
        ILogger<SpotAttachmentService> logger)
    {
        _dbContextFactory = dbContextFactory;
        _nntpClientPool = nntpClientPool;
        _logger = logger;
    }

    public async Task<MemoryStream?> RetrieveNzb(int spotId, CancellationToken cancellationToken)
    {
        await using var dbContext = await _dbContextFactory.CreateDbContextAsync(cancellationToken);
        var spot = await dbContext.Spots.FirstOrDefaultAsync(s => s.Id == spotId, cancellationToken);
        if (spot == null || string.IsNullOrEmpty(spot.NzbMessageId))
            return null;

        try
        {
            using var lease = await _nntpClientPool.GetLease();
            var nzbMessageId = spot.NzbMessageId;

            // Fetch the article headers which contains the NZB payload
            var nzbArticleResponse = lease.Client.Article(new NntpMessageId(nzbMessageId));
            if (!nzbArticleResponse.Success)
            {
                _logger.CouldNotRetrieveArticle(spot.MessageId, nzbArticleResponse.Code, nzbArticleResponse.Message);
                return null;
            }

            var nzbData = string.Concat(nzbArticleResponse.Article.Body);
            return await NzbArticleParser.Parse(nzbData, cancellationToken);
        }
        catch (NntpException ex)
        {
            _logger.CouldNotRetrieveArticle(ex, spot.MessageId);
            return null;
        }
    }

    public Task<MemoryStream?> RetrieveImage(int spotId, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}