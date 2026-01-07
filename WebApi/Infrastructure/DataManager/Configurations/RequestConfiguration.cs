using Domain.Entities;
using Infrastructure.Common;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataManager.Configurations;

public class RequestConfiguration : BaseEntityConfiguration<Request>
{
    public override void Configure(EntityTypeBuilder<Request> builder)
    {
        base.Configure(builder);

        builder.Property(w => w.Query)
        .IsRequired().HasMaxLength(255);

        builder.HasOne(w => w.ServerRef)
        .WithMany(w => w.RequestsRefs)
        .HasForeignKey(w => w.ServerId);
    }
}