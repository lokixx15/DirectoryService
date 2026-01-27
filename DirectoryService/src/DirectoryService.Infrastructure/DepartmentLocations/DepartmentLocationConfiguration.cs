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
            .HasColumnName("fk_departmentlocation_department_id");

        builder
            .Property(d => d.LocationId)
            .HasColumnName("fk_departmentlocation_location_id");

        builder
            .HasOne<Department>()
            .WithMany(d => d.Locations)
            .HasForeignKey(d => d.DepartmentId)
            .OnDelete(DeleteBehavior.NoAction);

        builder
            .HasOne<Location>()
            .WithMany(l => l.Departments)
            .HasForeignKey(d => d.LocationId)
            .OnDelete(DeleteBehavior.NoAction);
    }
}