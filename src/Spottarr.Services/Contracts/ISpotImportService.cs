namespace Spottarr.Services.Contracts;

public interface ISpotImportService
{
    public Task Import();
    Task<MemoryStream?> RetrieveNzb(int spotId);
    Task<MemoryStream?> RetrieveImage(int spotId);
}