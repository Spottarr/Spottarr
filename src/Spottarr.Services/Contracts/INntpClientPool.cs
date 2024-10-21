using Spottarr.Services.Nntp;

namespace Spottarr.Services.Contracts;

internal interface INntpClientPool
{
    Task<NntpClientWrapper> BorrowClient();
    void ReturnClient(NntpClientWrapper client);
}