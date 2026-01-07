using Domain.Entities;
using Infrastructure.Common;
using Infrastructure.Identity.AspNetCoreIdentity;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataManager.Configurations;

public class ServerConfiguration : BaseEntityConfiguration<Server>
{
    public override void Configure(EntityTypeBuilder<Server> builder)
    {
        base.Configure(builder);

        builder.Property(w => w.Name)
        .IsRequired().HasMaxLength(255);

        builder.Property(w => w.Address)
        .IsRequired();
        
        builder.HasOne<ApplicationUser>()
        .WithMany(w => w.ServerRefs)
        .HasForeignKey(w => w.UserId);
    }
}