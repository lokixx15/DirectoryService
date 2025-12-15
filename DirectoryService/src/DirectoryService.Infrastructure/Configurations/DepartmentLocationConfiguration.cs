using DirectoryService.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DirectoryService.Infrastructure.Configurations
{
    public class DepartmentLocationConfiguration : IEntityTypeConfiguration<DepartmentLocation>
    {
        public void Configure(EntityTypeBuilder<DepartmentLocation> builder)
        {
            builder.ToTable("department_location");

            builder
                .HasKey(d => d.Id)
                .HasName("pk_departmetntlocation_id");

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
}