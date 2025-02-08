namespace Spottarr.Services.Contracts;

public interface ISpotImportService
{
    public Task<MemoryStream?> RetrieveNzb(int spotId);
    public Task<MemoryStream?> RetrieveImage(int spotId);
    public Task Import(CancellationToken cancellationToken);
}