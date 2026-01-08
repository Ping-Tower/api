using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Infrastructure.IdentityManager.AspNetCoreIdentity;

namespace Infrastructure.Identity.AspNetCoreIdentity;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.HasKey(w => w.Id);

        builder.Property(w => w.UpdatedAt)
        .IsRequired(false);

        builder.Property(w => w.UpdatedById)
        .IsRequired(false);

        builder.HasMany(w => w.ServerRefs);

        builder.HasIndex(w => w.Id);
        builder.HasIndex(w => w.UserName);
    }
}