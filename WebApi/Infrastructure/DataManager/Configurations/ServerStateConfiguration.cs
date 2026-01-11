using Domain.Entities;
using Infrastructure.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataManager.Configurations;

public class ServerStateConfiguration : BaseEntityConfiguration<ServerState>
{
    public override void Configure(EntityTypeBuilder<ServerState> builder)
    {
        base.Configure(builder);

        builder.Property(w => w.IsUp)
        .IsRequired(true).HasDefaultValue(true);

        builder.Property(w => w.FailCount)
        .IsRequired(false);
        
        builder.HasOne(w => w.ServerRef)
        .WithOne(w => w.ServerStateRef)
        .HasForeignKey<ServerState>(w => w.ServerId);
    }
}