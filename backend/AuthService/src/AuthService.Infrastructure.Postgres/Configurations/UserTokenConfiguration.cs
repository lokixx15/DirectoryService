using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Postgres.Configurations;

public class UserTokenConfiguration : IEntityTypeConfiguration<IdentityUserToken<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityUserToken<Guid>> builder)
    {
        builder.ToTable("user_tokens");

        builder.Property(uT => uT.Name).HasColumnName("name");
        builder.Property(uT => uT.Value).HasColumnName("value");
        builder.Property(uT => uT.LoginProvider).HasColumnName("login_provider");
        builder.Property(uT => uT.UserId).HasColumnName("user_id");
    }
}
