using DirectoryService.Domain;
using DirectoryService.Domain.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Locations;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("locations");

        builder
            .HasKey(l => l.Id)
            .HasName("pk_location_id");

        builder
            .Property(l => l.Id)
            .HasColumnName("id");

        builder
            .ComplexProperty(l => l.Name, bp =>
            {
                bp.Property(l => l.Value)
                    .HasMaxLength(Constants.MAX_LOCATION_NAME_LENGTH)
                    .IsRequired()
                    .HasColumnName("name");
            });

        builder.OwnsOne(l => l.Address, bp =>
        {
            bp.ToJson("address");
            bp.Ignore(a => a.FullAddress);
        });

        builder
            .ComplexProperty(l => l.Timezone, bp =>
            {
                bp.Property(l => l.Value)
                    .HasMaxLength(Constants.MAX_LOCATION_TIMEZONE_LENGTH)
                    .IsRequired()
                    .HasColumnName("timezone");
            });

        builder
            .Property(l => l.IsActive)
            .IsRequired()
            .HasColumnName("is_active");

        builder
            .Property(d => d.CreatedAt)
            .IsRequired()
            .HasColumnName("created_at")
            .HasColumnType("timestamp with time zone");

        builder
            .Property(d => d.UpdatedAt)
            .IsRequired()
            .HasColumnName("updated_at")
            .HasColumnType("timestamp with time zone");

        builder
            .Property(d => d.DeletedAt)
            .HasColumnName("deleted_at")
            .HasColumnType("timestamp with time zone");
    }
}