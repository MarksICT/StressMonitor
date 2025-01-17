using DataCollection.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataCollection.Persistence.Configurations;

public class BrowserDataConfiguration : IEntityTypeConfiguration<BrowserDataEntity>
{
    public void Configure(EntityTypeBuilder<BrowserDataEntity> builder)
    {
        builder.HasKey(data => data.Id);
        builder.Property(data => data.TabId);
        builder.Property(data => data.Uri).HasConversion(uri => uri == null ? null : uri.AbsoluteUri,
            s => s == null ? null : new Uri(s), ValueComparer.CreateDefault<Uri>(true));
        builder.Property(data => data.StartTime);
        builder.Property(data => data.StopTime);
    }
}