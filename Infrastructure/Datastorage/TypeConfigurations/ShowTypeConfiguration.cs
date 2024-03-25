using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Showtime.Infrastructure.Datastorage.TypeConfigurations;
internal class ShowTypeConfiguration : IEntityTypeConfiguration<Show>
{
    public void Configure(EntityTypeBuilder<Show> builder)
    {
        builder.HasKey(s => s.Id);
        builder.Property(s => s.Id).ValueGeneratedNever();

        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(s => s.Language)
            .IsRequired()
            .HasMaxLength(20);

        builder.Property(s => s.Genres)
            .HasMaxLength(20);

        builder.Property(s => s.Summary)
            .HasMaxLength(3000);

        builder.Property(s => s.Premiered)
            .HasConversion<DateOnlyConverter>()
            .HasColumnType("date");
            
        builder.Property(s => s.Genres)
            .HasConversion(
                v => JsonSerializer.Serialize(v, (JsonSerializerOptions)null!),
                v => JsonSerializer.Deserialize<List<string>>(v, (JsonSerializerOptions)null!)!,
                //ICollection<string> represents a mutable reference type.
                //This means that a ValueComparer<T> is needed so that EF Core can track and detect changes correctly.\
                //(https://learn.microsoft.com/en-us/ef/core/modeling/value-conversions?tabs=data-annotations)
                    new ValueComparer<IReadOnlyCollection<string>>(
                        (c1, c2) => c1!.SequenceEqual(c2!),
                        c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                        c => c.ToList()));
    }
}

