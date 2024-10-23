namespace Spottarr.Data.Helpers;

public static class DbPathHelper
{
    public static string GetDbPath() =>
        Path.Join(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "spottarr.db");
}