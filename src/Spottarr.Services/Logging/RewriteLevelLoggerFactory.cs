using Microsoft.Extensions.Logging;

namespace Spottarr.Services.Logging;

internal class RewriteLevelLoggerFactory : ILoggerFactory
{
    private readonly ILoggerFactory _loggerFactory;

    public RewriteLevelLoggerFactory(ILoggerFactory loggerFactory) => _loggerFactory = loggerFactory;

    public ILogger CreateLogger(string categoryName)
    {
        var logger = _loggerFactory.CreateLogger(categoryName);

        if (categoryName.StartsWith("Spottarr.Usenet.", StringComparison.OrdinalIgnoreCase))
        {
            return new RewriteLevelLogger(logger, l => l switch
            {
                LogLevel.Information => LogLevel.Debug,
                _ => l
            });
        }

        return logger;
    }

    public void AddProvider(ILoggerProvider provider) => _loggerFactory.AddProvider(provider);

    public void Dispose() => _loggerFactory.Dispose();
}