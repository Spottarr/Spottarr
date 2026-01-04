namespace Spottarr.Services.Contracts;

public interface ISpotAttachmentService
{
    public Task<MemoryStream?> RetrieveNzb(int spotId, CancellationToken cancellationToken);
    public Task<MemoryStream?> RetrieveImage(int spotId, CancellationToken cancellationToken);
}