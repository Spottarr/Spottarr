using Spottarr.Services.Helpers;

namespace Spottarr.Web.Logging;

public static class LoggingBuilderExtensions
{
    public static ILoggingBuilder AddConsole(this ILoggingBuilder builder, IHostEnvironment environment) =>
        environment.IsContainer()
            ? builder.AddSimpleConsole(options =>
            {
                options.SingleLine = true;
                options.TimestampFormat = "HH:mm:ss ";
            })
            : builder.AddConsole();
}