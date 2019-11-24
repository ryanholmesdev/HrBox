using HrBoxApi.Models.DB;
using Microsoft.EntityFrameworkCore;

namespace HrBoxApi.Data
{
  public class AppDbContext : DbContext
  {
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<UserToken> UserTokens { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
      // User - Default createdutc date.
      modelBuilder.Entity<User>()
         .Property(u => u.CreatedUtc)
         .HasDefaultValueSql("getutcdate()");

      // User - Unique email column.
      modelBuilder.Entity<User>()
        .HasIndex(u => u.Email).IsUnique();

      // User - Foreign key to UserTokens
      modelBuilder.Entity<User>()
        .HasMany(c => c.UserTokens)
        .WithOne(e => e.User);
    }
  }
}
