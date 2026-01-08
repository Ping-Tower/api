using Domain.Entities;
using Infrastructure.Common;
using Infrastructure.IdentityManager.AspNetCoreIdentity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataManager.Configurations;

public class ServerConfiguration : BaseEntityConfiguration<Server>
{
    public override void Configure(EntityTypeBuilder<Server> builder)
    {
        base.Configure(builder);

        builder.Property(w => w.Name)
        .IsRequired().HasMaxLength(255);

        builder.Property(w => w.Host)
        .IsRequired().HasMaxLength(255);

        builder.Property(w => w.Port)
        .IsRequired(false);

        builder.Property(w => w.CheckIntervalSec)
        .IsRequired().HasDefaultValue(60);

        builder.Property(w => w.IsActive)
        .IsRequired().HasDefaultValue(true);

        builder.HasOne<ApplicationUser>()
        .WithMany(w => w.ServerRefs)
        .HasForeignKey(w => w.UserId);
    }
}