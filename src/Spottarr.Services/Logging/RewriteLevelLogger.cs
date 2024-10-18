using Microsoft.Extensions.Logging;

namespace Spottarr.Services.Logging;

public class RewriteLevelLogger : ILogger
{
    private readonly ILogger _logger;
    private readonly Func<LogLevel, LogLevel> _rewriteFunc;

    public RewriteLevelLogger(ILogger logger, Func<LogLevel, LogLevel> rewriteFunc)
    {
        _logger = logger;
        _rewriteFunc = rewriteFunc;
    }

    public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception? exception, Func<TState, Exception?, string> formatter) =>
        _logger.Log(_rewriteFunc.Invoke(logLevel), eventId, state, exception, formatter);

    public bool IsEnabled(LogLevel logLevel) => _logger.IsEnabled(logLevel);

    public IDisposable? BeginScope<TState>(TState state) where TState : notnull => _logger.BeginScope(state);
}