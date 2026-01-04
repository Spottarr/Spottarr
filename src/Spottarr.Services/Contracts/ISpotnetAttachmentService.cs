namespace Spottarr.Services.Contracts;

public interface ISpotnetAttachmentService
{
    public Task<MemoryStream?> RetrieveNzb(int spotId, CancellationToken cancellationToken);
    public Task<MemoryStream?> RetrieveImage(int spotId, CancellationToken cancellationToken);
}