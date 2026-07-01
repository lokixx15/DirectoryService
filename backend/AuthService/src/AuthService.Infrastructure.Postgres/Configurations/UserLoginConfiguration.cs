using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace AuthService.Infrastructure.Postgres.Configurations;

public class UserLoginConfiguration : IEntityTypeConfiguration<IdentityUserLogin<Guid>>
{
    public void Configure(EntityTypeBuilder<IdentityUserLogin<Guid>> builder)
    {
        builder.ToTable("user_logins");

        builder.Property(uL => uL.LoginProvider).HasColumnName("login_provider");
        builder.Property(uL => uL.ProviderDisplayName).HasColumnName("provider_display_name");
        builder.Property(uL => uL.ProviderKey).HasColumnName("provider_key");
        builder.Property(uL => uL.UserId).HasColumnName("user_id");
    }
}
