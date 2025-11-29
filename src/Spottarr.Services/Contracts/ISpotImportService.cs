namespace Spottarr.Services.Contracts;

public interface ISpotImportService
{
    public Task<MemoryStream?> RetrieveNzb(int spotId, CancellationToken cancellationToken);
    public Task<MemoryStream?> RetrieveImage(int spotId, CancellationToken cancellationToken);
    public Task Import(CancellationToken cancellationToken);
}