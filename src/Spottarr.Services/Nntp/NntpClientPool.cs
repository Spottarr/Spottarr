using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using Spottarr.Services.Configuration;
using Spottarr.Services.Contracts;

namespace Spottarr.Services.Nntp;

internal class NntpClientPool : INntpClientPool, IDisposable
{
    private readonly IOptions<UsenetOptions> _usenetOptions;
    private readonly int _maxPoolSize;
    
    private readonly ConcurrentBag<NntpClientWrapper> _availableClients = new();
    private readonly SemaphoreSlim _semaphore;
    
    private int _currentSize;
    private bool _disposed;

    public NntpClientPool(IOptions<UsenetOptions> usenetOptions)
    {
        _usenetOptions = usenetOptions;
        _maxPoolSize = usenetOptions.Value.MaxConnections;
        _semaphore = new SemaphoreSlim(_maxPoolSize, _maxPoolSize);
    }
    
    public async Task<NntpClientWrapper> BorrowClient()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        await _semaphore.WaitAsync();

        if (_availableClients.TryTake(out var client))
            return client;

        if (_currentSize > _maxPoolSize)
            throw new InvalidOperationException("No available clients in the pool.");

        client = await CreateClient();
        Interlocked.Increment(ref _currentSize);
        return client;
    }
    
    public void ReturnClient(NntpClientWrapper client)
    {
        client.ResetCounters();
        ObjectDisposedException.ThrowIf(_disposed, this);

        _availableClients.Add(client);
        _semaphore.Release();
    }

    private async Task<NntpClientWrapper> CreateClient()
    {
        var options = _usenetOptions.Value;
        var client = new NntpClientWrapper();
        var success = await client.ConnectAndAuthenticateAsync(options.Hostname, options.Port, options.UseTls, options.Username, options.Password);
        if (!success) throw new InvalidOperationException($"Failed to connect to '{options.Hostname}'");
        return client;
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (_disposed) return;

        if (disposing)
        {
            foreach (var client in _availableClients)
            {
                client.Dispose();
            }

            _availableClients.Clear();
            _semaphore.Dispose();
        }

        _disposed = true;
    }
}