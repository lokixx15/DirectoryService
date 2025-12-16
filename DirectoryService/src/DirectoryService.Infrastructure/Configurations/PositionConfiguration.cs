using DirectoryService.Domain;
using DirectoryService.Domain.Entities;
using DirectoryService.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Configurations
{
    public class PositionConfiguration : IEntityTypeConfiguration<Position>
    {
        public void Configure(EntityTypeBuilder<Position> builder)
        {
            builder.ToTable("positions");

            builder
                .HasKey(p => p.Id)
                .HasName("pk_positon_id");

            builder
                .ComplexProperty(p => p.Name, bp =>
                {
                    bp.Property(p => p.Value)
                        .HasMaxLength(Constants.MAX_POSITION_NAME_LENGTH)
                        .IsRequired()
                        .HasColumnName("name");
                });

            builder
                .Property(l => l.Description)
                .HasMaxLength(1000)
                .IsRequired()
                .HasColumnName("description");

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
}