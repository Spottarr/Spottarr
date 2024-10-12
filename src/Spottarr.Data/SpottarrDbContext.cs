using Microsoft.EntityFrameworkCore;

namespace Spottarr.Data;

public class SpottarrDbContext : DbContext
{
    private readonly string _dbPath;
    
    public SpottarrDbContext()
    {
        var path = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        _dbPath = Path.Join(path, "spottarr.db");
    }
    
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        => optionsBuilder.UseSqlite($"Data Source={_dbPath}");
}