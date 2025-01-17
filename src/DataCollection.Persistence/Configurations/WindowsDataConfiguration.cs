using DataCollection.Persistence.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DataCollection.Persistence.Configurations;

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
