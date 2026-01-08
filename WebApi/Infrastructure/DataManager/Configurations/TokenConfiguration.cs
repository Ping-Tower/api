using Domain.Entities;
using Infrastructure.Common;
using Infrastructure.IdentityManager.AspNetCoreIdentity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataManager.Configurations;

public class TokenConfiguration : BaseEntityConfiguration<Token>
{
    public override void Configure(EntityTypeBuilder<Token> builder)
    {
        base.Configure(builder);

        builder.HasOne<ApplicationUser>()
        .WithMany(w => w.TokenRefs)
        .HasForeignKey(w => w.UserId);

        builder.Property(w => w.RefreshToken)
        .IsRequired();

        builder.Property(w => w.Expire)
        .IsRequired();
    }
}