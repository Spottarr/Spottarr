using Spottarr.Services.Models;

namespace Spottarr.Services.Contracts;

public interface ISpotnetAttachmentService
{
    public Task<SpotAttachmentResponse?> FetchNzb(int spotId, CancellationToken cancellationToken);
    public Task<SpotAttachmentResponse?> FetchImage(int spotId, CancellationToken cancellationToken);
}