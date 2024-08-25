using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using TodoApp.Domain.RefreshTokenAggregate;
using TodoApp.Domain.UserAggregate;

namespace TodoApp.Infrastructure.Configs;

public sealed class ApplicationUserConfig : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.ToTable("ApplicationUsers");

        builder.OwnsMany<RefreshToken>("_refreshTokens", rb =>
        {
            rb.Property(r => r.Token).IsRequired();
        });

        builder.Property(a => a.FirstName).HasMaxLength(50).IsRequired();
        builder.Property(a => a.LastName).HasMaxLength(50).IsRequired();
        builder.Ignore(a => a.RefreshTokens);
    }
}