using Domain.Entities;
using Infrastructure.DataManager.Configurations;
using Infrastructure.IdentityManager.AspNetCoreIdentity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataManager.Contexts;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    { }

    public DbSet<Server> Servers { get; set; }
    public DbSet<Token> Tokens { get; set; }
    public DbSet<NotificationSettings> NotificationSettings {get; set;}
    public DbSet<PingSettings> PingSettings {get; set;}
    public DbSet<TelegramAccount> TelegramAccount {get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.ApplyConfiguration(new ServerConfiguration());
        builder.ApplyConfiguration(new TokenConfiguration());
        builder.ApplyConfiguration(new NotificationSettingsConfiguration());
        builder.ApplyConfiguration(new PingSettingsConfiguration());
        builder.ApplyConfiguration(new TelegramAccountConfiguration());
    }
}
