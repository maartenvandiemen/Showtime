using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Showtime.Infrastructure.Datastorage.TypeConfigurations;
internal class SyncStatusTypeConfiguration : IEntityTypeConfiguration<SyncStatus>
{
    public void Configure(EntityTypeBuilder<SyncStatus> builder)
    {
        builder.HasKey(s => s.Pagenumber);
        builder.Property(s => s.Pagenumber).ValueGeneratedNever();
    }
}
