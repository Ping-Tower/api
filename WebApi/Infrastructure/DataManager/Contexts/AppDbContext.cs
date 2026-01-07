using Domain.Entities;
using Infrastructure.DataManager.Configurations;
using Infrastructure.Identity.AspNetCoreIdentity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.DataManager.Contexts;

public class AppDbContext : IdentityDbContext<ApplicationUser>
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    { }

    public DbSet<Server> Serevers { get; set; }
    public DbSet<Request> Requests { get; set; }
    public DbSet<Token> Tokens { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.ApplyConfiguration(new RequestConfiguration());
        builder.ApplyConfiguration(new ServerConfiguration());
        builder.ApplyConfiguration(new TokenConfiguration());
    }
}