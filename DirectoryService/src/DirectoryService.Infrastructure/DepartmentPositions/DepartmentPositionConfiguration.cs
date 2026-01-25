using DirectoryService.Domain.DepartmentPositions;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Positions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.DepartmentPositions;

public class DepartmentPositionConfiguration : IEntityTypeConfiguration<DepartmentPosition>
{
    public void Configure(EntityTypeBuilder<DepartmentPosition> builder)
    {
        builder.ToTable("department_position");

        builder
            .HasKey(d => d.Id)
            .HasName("pk_departmentposition_id");

        builder
            .Property(d => d.Id)
            .HasColumnName("id");

        builder
            .Property(d => d.DepartmentId)
            .HasColumnName("fk_departmentposition_department_id");

        builder
            .Property(d => d.PositionId)
            .HasColumnName("fk_departmentposition_position_id");

        builder
            .HasOne<Department>()
            .WithMany(d => d.Positions)
            .HasForeignKey(d => d.DepartmentId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasOne<Position>()
            .WithMany(p => p.Departments)
            .HasForeignKey(d => d.PositionId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}