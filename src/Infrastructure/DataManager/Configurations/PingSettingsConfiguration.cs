using Domain.Entities;
using Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataManager.Configurations;

public class PingSettingsConfiguration : BaseEntityConfiguration<PingSettings>
{
    public override void Configure(EntityTypeBuilder<PingSettings> builder)
    {
        base.Configure(builder);

        builder.Property(w => w.IntervalSec)
        .IsRequired(true).HasDefaultValue(60);

        builder.Property(w => w.LatencyThresholdMs)
        .IsRequired(true).HasDefaultValue(400);

        builder.Property(w => w.Retries)
        .IsRequired(true).HasDefaultValue(0);

        builder.Property(w => w.FailureThreshold)
        .IsRequired(true).HasDefaultValue(1);

        builder.HasOne(w => w.ServerRef)
        .WithOne(w => w.PingSettingsRef)
        .HasForeignKey<PingSettings>(w => w.ServerId);
    }
}
