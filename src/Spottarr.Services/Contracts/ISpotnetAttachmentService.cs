namespace Spottarr.Services.Contracts;

public interface ISpotnetAttachmentService
{
    public Task<MemoryStream?> FetchNzb(int spotId, CancellationToken cancellationToken);
    public Task<MemoryStream?> FetchImage(int spotId, CancellationToken cancellationToken);
}