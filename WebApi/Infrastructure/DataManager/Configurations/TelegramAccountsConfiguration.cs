using Domain.Entities;
using Infrastructure.Common;
using Infrastructure.IdentityManager.AspNetCoreIdentity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.DataManager.Configurations;

public class TelegramAccountsConfiguration : BaseEntityConfiguration<TelegramAccount>
{
    public override void Configure(EntityTypeBuilder<TelegramAccount> builder)
    {
        base.Configure(builder);

        builder.Property(w => w.ChatId)
        .IsRequired().HasMaxLength(255);

        builder.HasOne<ApplicationUser>()
        .WithOne(w => w.TelegramAccountRef)
        .HasForeignKey<TelegramAccount>(w => w.UserId);
    }
}