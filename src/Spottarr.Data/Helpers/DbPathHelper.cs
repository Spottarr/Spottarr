namespace Spottarr.Data.Helpers;

public static class DbPathHelper
{
    public static string GetDbPath()
    {
        var isContainer = Environment.GetEnvironmentVariable("DOTNET_RUNNING_IN_CONTAINER") == "true";
        var root = isContainer ? "/data" : Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return Path.Join(root, "spottarr.db");
    }
}