using System.Collections.Concurrent;
using Spottarr.Services.Configuration;
using Usenet.Nntp;

namespace Spottarr.Services.Nntp;

internal class NntpClientPool : IDisposable
{
    private readonly string _hostname;
    private readonly int _port;
    private readonly bool _useTls;
    private readonly string _username;
    private readonly string _password;
    private readonly int _maxPoolSize;
    
    private readonly ConcurrentBag<NntpClientWrapper> _availableClients = new();
    private readonly SemaphoreSlim _semaphore;
    
    private int _currentSize;
    private bool _disposed;

    public NntpClientPool(string hostname, int port, bool useTls, string username, string password, int maxPoolSize)
    {
        _hostname = hostname;
        _port = port;
        _useTls = useTls;
        _username = username;
        _password = password;
        _maxPoolSize = maxPoolSize;
        _semaphore = new SemaphoreSlim(maxPoolSize, maxPoolSize);
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
        var client = new NntpClientWrapper();
        var success = await client.ConnectAndAuthenticateAsync(_hostname, _port, _useTls, _username, _password);
        if (!success) throw new InvalidOperationException($"Failed to connect to '{_hostname}'");
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