using Domain.Entities;
using Infrastructure.Common;
using Infrastructure.IdentityManager.AspNetCoreIdentity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataManager.Configurations;

public class NotificationSettingsConfiguration : BaseEntityConfiguration<NotificationSettings>
{
    public override void Configure(EntityTypeBuilder<NotificationSettings> builder)
    {
        base.Configure(builder);

        builder.Property(w => w.OnUp)
        .IsRequired(true).HasDefaultValue(true);

        builder.Property(w => w.OnDown)
        .IsRequired(true).HasDefaultValue(true);

        builder.Property(w => w.OnLatency)
        .IsRequired(true).HasDefaultValue(true);

        builder.Property(w => w.CooldownSec)
        .IsRequired(true).HasDefaultValue(600);

        builder.HasOne<ApplicationUser>()
        .WithOne(w => w.NotificationSettingsRef)
        .HasForeignKey<NotificationSettings>(w => w.UserId);
    }
} 
