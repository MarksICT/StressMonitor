using System.ComponentModel.DataAnnotations;
using DataCollection.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataCollection.Persistence;

public class DataCollectorDbContext : DbContext
{
    public DbSet<WindowsDataEntity> WindowsData { get; set; } = null!;
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlite($"Data Source={Path.Combine(AppContext.BaseDirectory, "DataCollection.db")}");
    }
}

public class WindowsDataConfiguration : IEntityTypeConfiguration<WindowsDataEntity>
{
    public void Configure(EntityTypeBuilder<WindowsDataEntity> builder)
    {
        builder.HasKey(data => data.Id);
        builder.Property(data => data.WindowTitle);
        builder.Property(data => data.ProcessFileName);
        builder.Property(data => data.ProcessFriendlyName);
        builder.Property(data => data.StartTime);
        builder.Property(data => data.StopTime);
    }
}

public class WindowsDataEntity
{
    public Guid Id { get; init; }
    [MaxLength(1000)]
    public string? WindowTitle { get; init; }
    [MaxLength(1000)]
    public string? ProcessFileName { get; init; }
    [MaxLength(1000)]
    public string? ProcessFriendlyName { get; init; }
    public DateTimeOffset StartTime { get; init; }
    public DateTimeOffset StopTime { get; init; }
}