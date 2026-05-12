using DirectoryService.Domain.DepartmentLocations;
using DirectoryService.Domain.Departments;
using DirectoryService.Domain.Locations;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.DepartmentLocations;

public class DepartmentLocationConfiguration : IEntityTypeConfiguration<DepartmentLocation>
{
    public void Configure(EntityTypeBuilder<DepartmentLocation> builder)
    {
        builder.ToTable("department_location");

        builder
            .HasKey(d => d.Id)
            .HasName("pk_departmetntlocation_id");

        builder
            .Property(d => d.Id)
            .HasColumnName("id");

        builder
            .Property(d => d.DepartmentId)
            .HasColumnName("department_id");

        builder
            .Property(d => d.LocationId)
            .HasColumnName("location_id");

        builder
            .HasOne<Department>()
            .WithMany(d => d.Locations)
            .HasForeignKey(d => d.DepartmentId)
            .HasConstraintName("fk_departmentlocation_department_id")
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasOne<Location>()
            .WithMany(l => l.Departments)
            .HasForeignKey(d => d.LocationId)
            .HasConstraintName("fk_departmentlocation_location_id")
            .OnDelete(DeleteBehavior.NoAction);
    }
}