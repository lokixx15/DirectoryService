using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Postgres.Configurations;

public class RoleClaimConfiguration : IEntityTypeConfiguration<IdentityRoleClaim<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityRoleClaim<Guid>> builder)
    {
        builder.ToTable("role_claims");

        builder.Property(rC => rC.Id).HasColumnName("id");
        builder.Property(rC => rC.RoleId).HasColumnName("role_id");
        builder.Property(rC => rC.ClaimType).HasColumnName("claim_type");
        builder.Property(rC => rC.ClaimValue).HasColumnName("claim_value");
    }
}
