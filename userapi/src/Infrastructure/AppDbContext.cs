using Microsoft.EntityFrameworkCore;
using UserApi.Domain.Entities;

namespace UserApi.Infrastructure
{
    public class AppDbContext : DbContext
    {
        public DbSet<User> Users { get; set; } = null!;

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(b =>
            {
                b.HasKey(u => u.Id);
                b.Property(u => u.Email).IsRequired();
                b.Property(u => u.Name).IsRequired();
                b.Property(u => u.PasswordHash).IsRequired();
                b.HasIndex(u => u.Email).IsUnique();
            });
        }
    }
}
