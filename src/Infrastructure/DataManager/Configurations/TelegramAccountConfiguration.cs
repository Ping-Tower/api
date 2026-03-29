using Domain.Entities;
using Infrastructure.Common;
using Infrastructure.IdentityManager.AspNetCoreIdentity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataManager.Configurations;

public class TelegramAccountConfiguration : BaseEntityConfiguration<TelegramAccount>
{
    public override void Configure(EntityTypeBuilder<TelegramAccount> builder)
    {
        base.Configure(builder);

        builder.Property(w => w.TelegramUserId)
        .IsRequired();

        builder.Property(w => w.FirstName)
        .IsRequired()
        .HasMaxLength(255);

        builder.Property(w => w.Username)
        .HasMaxLength(255);

        builder.Property(w => w.PhotoUrl)
        .HasMaxLength(2048);

        builder.Property(w => w.AuthDateUtc)
        .IsRequired();

        builder.HasIndex(w => w.TelegramUserId)
        .IsUnique();

        builder.HasOne<ApplicationUser>()
        .WithOne(w => w.TelegramAccountRef)
        .HasForeignKey<TelegramAccount>(w => w.UserId);
    }
}
