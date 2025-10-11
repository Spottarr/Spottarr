namespace Spottarr.Data.Helpers;

public static class DbPathHelper
{
    public static string GetDbPath()
    {
        var isContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
        var root = isContainer
            ? "/data"
            : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "spottarr");

        var path = Path.Join(root, "spottarr.db");

        try
        {
            // Ensure that the data directory exists
            if (!Directory.Exists(root)) Directory.CreateDirectory(root);

            // Ensure we can write to it
            var probe = Path.Join(root, $".probe-{Guid.NewGuid():N}");
            using var _ = File.Create(probe, 1, FileOptions.DeleteOnClose);
        }
        catch (Exception ex) when (ex is UnauthorizedAccessException or IOException)
        {
            throw new InvalidOperationException(
                $"Spottarr can not be started. The database path '{path}' is not writeable. " +
                "Please ensure that the configured user/group has write permissions");
        }

        return path;
    }
}