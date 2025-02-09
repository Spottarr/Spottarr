using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Spottarr.Services.Configuration;
using Spottarr.Services.Contracts;
using Spottarr.Services.Logging;

namespace Spottarr.Services.Nntp;

internal class NntpClientPool : INntpClientPool, IDisposable
{
    private readonly object _lock = new();
    private readonly Queue<NntpClientWrapper> _availableClients = [];
    private readonly TimeSpan _monitorInterval = TimeSpan.FromSeconds(10);
    private readonly TimeSpan _idleTimeout = TimeSpan.FromSeconds(30);
    private readonly TimeSpan _waitTimeout = TimeSpan.FromSeconds(60);
    private readonly CancellationTokenSource _cts = new();

    private readonly ILogger<NntpClientPool> _logger;
    private readonly IOptions<UsenetOptions> _usenetOptions;
    private readonly int _maxPoolSize;
    private readonly SemaphoreSlim _semaphore;

    private int _currentPoolSize;
    private bool _disposed;

    public NntpClientPool(IHostEnvironment hostEnvironment, ILoggerFactory loggerFactory, ILogger<NntpClientPool> logger, IOptions<UsenetOptions> usenetOptions)
    {
        _logger = logger;
        _usenetOptions = usenetOptions;
        _maxPoolSize = usenetOptions.Value.MaxConnections;
        _semaphore = new SemaphoreSlim(_maxPoolSize, _maxPoolSize);
        
        // Enable NNTP client logging
        if (hostEnvironment.IsDevelopment())
            Usenet.Logger.Factory = loggerFactory;

        // Start the background monitoring task
        Task.Run(() => MonitorIdleClients(_cts.Token));
    }

    public async Task<NntpClientWrapper> BorrowClient()
    {
        ObjectDisposedException.ThrowIf(_disposed, this);

        var success = await _semaphore.WaitAsync(_waitTimeout);
        if (!success) throw new InvalidOperationException("Timed out waiting for NNTP (usenet) client");

        _logger.BorrowingNntpClient();
        var client = BorrowClientInternal();
        if (client.Connected) return client;

        var options = _usenetOptions.Value;
        var (connected, authenticated) = await client.ConnectAndAuthenticateAsync(options.Hostname, options.Port,
            options.UseTls, options.Username, options.Password);
        if (!connected || !authenticated)
            throw new InvalidOperationException(
                $"Failed to connect to '{options.Hostname}:{options.Port}' TLS={options.UseTls} C={connected} A={authenticated}.'");
        return client;
    }

    public void ReturnClient(NntpClientWrapper client)
    {
        ObjectDisposedException.ThrowIf(_disposed, this);
        client.ResetCounters();
        _logger.ReturningNntpClient();

        lock (_lock)
        {
            _availableClients.Enqueue(client);
            _semaphore.Release();
        }
    }

    private NntpClientWrapper BorrowClientInternal()
    {
        lock (_lock)
        {
            if (_availableClients.TryDequeue(out var client))
                return client;

            if (_currentPoolSize > _maxPoolSize)
                throw new InvalidOperationException("No available clients in the pool.");

            _currentPoolSize++;
            _logger.CreatingNewNntpClient(_currentPoolSize, _maxPoolSize);

            return new NntpClientWrapper();
        }
    }

    private async Task MonitorIdleClients(CancellationToken ct)
    {
        while (!ct.IsCancellationRequested)
        {
            await Task.Delay(_monitorInterval, ct);
            var now = DateTimeOffset.Now;

            lock (_lock)
            {
                var count = _availableClients.Count;
                for (var i = 0; i < count; i++)
                {
                    var client = _availableClients.Dequeue();
                    if (now - client.LastActivity > _idleTimeout)
                    {
                        client.Dispose();
                        _currentPoolSize--;
                        _logger.DisposingIdleNntpClient(_currentPoolSize, _maxPoolSize);
                        continue;
                    }

                    _availableClients.Enqueue(client);
                }
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

            lock (_lock)
            {
                foreach (var client in _availableClients)
                {
                    client.Dispose();
                }

                _availableClients.Clear();
            }

            _semaphore.Dispose();
            _cts.Dispose();
        }

        _disposed = true;
    }
}