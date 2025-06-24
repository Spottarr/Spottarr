namespace Spottarr.Data.Helpers;

public static class DbPathHelper
{
    public static string GetDbPath()
    {
        var isContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
        var root = isContainer
            ? "/data"
            : Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "spottarr");
        if (!Directory.Exists(root)) Directory.CreateDirectory(root);
        return Path.Join(root, "spottarr.db");
    }
}