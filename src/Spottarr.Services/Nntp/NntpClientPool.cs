using System.Collections.Concurrent;
using Microsoft.Extensions.Options;
using Spottarr.Services.Configuration;
using Spottarr.Services.Contracts;

namespace Spottarr.Services.Nntp;

internal class NntpClientPool : INntpClientPool, IDisposable
{
    private readonly ConcurrentBag<NntpClientWrapper> _availableClients = [];
    private readonly TimeSpan _monitorInterval = TimeSpan.FromSeconds(10);
    private readonly TimeSpan _idleTimeout = TimeSpan.FromSeconds(30);
    private readonly CancellationTokenSource _cts = new();
    
    private readonly IOptions<UsenetOptions> _usenetOptions;
    private readonly int _maxPoolSize;
    private readonly SemaphoreSlim _semaphore;

    private int _currentSize;
    private bool _disposed;
    
    public NntpClientPool(IOptions<UsenetOptions> usenetOptions)
    {
        _usenetOptions = usenetOptions;
        _maxPoolSize = usenetOptions.Value.MaxConnections;
        _semaphore = new SemaphoreSlim(_maxPoolSize, _maxPoolSize);
        
        // Start the background monitoring task
        Task.Run(() => MonitorIdleClients(_cts.Token));
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
        var (connected, authenticated) = await client.ConnectAndAuthenticateAsync(options.Hostname, options.Port, options.UseTls, options.Username, options.Password);
        if (!connected || !authenticated) throw new InvalidOperationException($"Failed to connect to '{options.Hostname}:{options.Port}' TLS={options.UseTls} C={connected} A={authenticated}.'");
        return client;
    }
    
    private async Task MonitorIdleClients(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            await Task.Delay(_monitorInterval, ct);
            
            var now = DateTimeOffset.Now;
            var activeClients = new List<NntpClientWrapper>();
            
            // Take all available clients from the pool
            while (_availableClients.TryTake(out var availableClient))
            {
                // Dispose idle clients
                if (now - availableClient.LastActivity > _idleTimeout)
                {
                    availableClient.Dispose();
                }
                else
                {
                    activeClients.Add(availableClient);
                }
            }

            // Add clients with recent activity back into the pool
            foreach (var activeClient in activeClients)
            {
                _availableClients.Add(activeClient);
            }
        }
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
            _cts.Cancel();

            foreach (var client in _availableClients)
            {
                client.Dispose();
            }

            _availableClients.Clear();
            _semaphore.Dispose();
            _cts.Dispose();
        }

        _disposed = true;
    }
}