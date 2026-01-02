using DirectoryService.Domain;
using DirectoryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Configurations;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("departments");

        builder
            .HasKey(d => d.Id)
            .HasName("pk_department_id");

        builder
            .ComplexProperty(d => d.Name, bp =>
            {
                bp.Property(d => d.Value)
                .HasMaxLength(Constants.MAX_DEPARTMENT_NAME_LENGTH)
                .IsRequired()
                .HasColumnName("name");
            });

        builder
            .Property(d => d.Identifier)
            .HasMaxLength(Constants.MAX_DEPARTMENT_IDENTIFIER_LENGTH)
            .IsRequired()
            .HasColumnName("identifier");

        builder
            .Property(d => d.ParentId)
            .HasColumnName("parent_id");

        builder
            .ComplexProperty(d => d.Path, bp =>
            {
                bp.Property(d => d.Value)
                    .HasMaxLength(Constants.MAX_DEPARTMENT_PATH_LENGTH)
                    .IsRequired()
                    .HasColumnName("path");
            });

        builder
            .Property(d => d.Depth)
            .IsRequired()
            .HasColumnName("depth");

        builder
            .Property(d => d.IsActive)
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
            .HasOne<Department>()
            .WithMany()
            .HasForeignKey(d => d.ParentId);
    }
}