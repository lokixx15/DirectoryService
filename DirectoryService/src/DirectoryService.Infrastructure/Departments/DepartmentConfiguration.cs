using DirectoryService.Domain;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Departments.VO;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Departments;

public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
{
    public void Configure(EntityTypeBuilder<Department> builder)
    {
        builder.ToTable("departments");

        builder
            .HasKey(d => d.Id)
            .HasName("pk_department_id");

        builder
            .Property(d => d.Id)
            .HasColumnName("id");

        builder
            .OwnsOne(d => d.Name, bp =>
            {
                bp.Property(d => d.Value)
                .HasMaxLength(Constants.MAX_DEPARTMENT_NAME_LENGTH)
                .IsRequired()
                .HasColumnName("name");
            });


        builder
            .OwnsOne(d => d.Identifier, bp =>
            {
                bp.Property(d => d.Value)
                .HasMaxLength(Constants.MAX_DEPARTMENT_IDENTIFIER_LENGTH)
                .IsRequired()
                .HasColumnName("identifier");
            });

        builder
            .Property(d => d.ParentId)
            .HasColumnName("parent_id");

        builder.Property(d => d.Path)
            .HasColumnName("path")
            .HasColumnType("ltree")
            .IsRequired()
            .HasConversion(
                value => value.Value,
                value => DepartmentPath.Create(value, null).Value);

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
            .WithMany(d => d.ChildrenDepartments)
            .HasForeignKey(d => d.ParentId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}