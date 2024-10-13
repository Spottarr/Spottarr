using Spottarr.Services.Configuration;
using Usenet.Nntp;

namespace Spottarr.Services.Nntp;

public class NntpClientHandler : IDisposable
{
    private readonly UsenetOptions _usenetOptions;
    private readonly NntpConnection _connection;
    private readonly NntpClient _client;
    private bool _disposed;
    private bool _connected;
    
    public NntpClient Client => _client;

    public NntpClientHandler(UsenetOptions usenetOptions)
    {
        _usenetOptions = usenetOptions;
        _connection = new NntpConnection();
        _client = new NntpClient(_connection);
    }

    public async Task<bool> ConnectAsync()
    {
        _connected = await _client.ConnectAsync(_usenetOptions.Hostname, _usenetOptions.Port, _usenetOptions.UseTls);
        if (!_connected) return false;
        var authenticated = _client.Authenticate(_usenetOptions.Username, _usenetOptions.Password);
        return authenticated && _connected;
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
            _client.Quit();
            _connection.Dispose();
        }

        _disposed = true;
    }
}