using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Postgres.Configurations;

public class UserClaimConfiguration : IEntityTypeConfiguration<IdentityUserClaim<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityUserClaim<Guid>> builder)
    {
        builder.ToTable("user_claims");

        builder.Property(uC => uC.Id).HasColumnName("id");
        builder.Property(uC => uC.UserId).HasColumnName("user_id");
        builder.Property(uC => uC.ClaimType).HasColumnName("claim_type");
        builder.Property(uC => uC.ClaimValue).HasColumnName("claim_value");
    }
}
