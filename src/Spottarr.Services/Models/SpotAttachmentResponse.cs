namespace Spottarr.Services.Models;

public sealed class SpotAttachmentResponse
{
    public required string FileName { get; init; }
    public required Stream Stream { get; init; }
}