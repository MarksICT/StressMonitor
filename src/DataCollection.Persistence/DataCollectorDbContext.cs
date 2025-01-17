using DataCollection.Persistence.Entities;
using Microsoft.EntityFrameworkCore;

namespace DataCollection.Persistence;

public class DataCollectorDbContext : DbContext
{
    public DbSet<WindowsDataEntity> WindowsData { get; set; } = null!;
    public DbSet<BrowserDataEntity> BrowserData { get; set; } = null!;

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        var dbLocation = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            "StressMonitor", "DataCollection.db");
        optionsBuilder.UseSqlite(
            $"Data Source={dbLocation}");
    }
}
