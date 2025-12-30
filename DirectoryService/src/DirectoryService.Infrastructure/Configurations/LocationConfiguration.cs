using DirectoryService.Domain;
using DirectoryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Configurations;

public class LocationConfiguration : IEntityTypeConfiguration<Location>
{
    public void Configure(EntityTypeBuilder<Location> builder)
    {
        builder.ToTable("locations");

        builder
            .HasKey(l => l.Id)
            .HasName("pk_location_id");

        builder
            .ComplexProperty(l => l.Name, bp =>
            {
                bp.Property(l => l.Value)
                    .HasMaxLength(Constants.MAX_LOCATION_NAME_LENGTH)
                    .IsRequired()
                    .HasColumnName("name");
            });

        builder
            .ComplexProperty(l => l.Address, bp =>
            {
                bp.Property(l => l.Value)
                    .HasMaxLength(Constants.MAX_LOCATION_ADDRESS_LENGTH)
                    .IsRequired()
                    .HasColumnName("address");
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
    }
}