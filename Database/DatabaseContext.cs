using Microsoft.EntityFrameworkCore;

namespace myMicroservice.Database
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Entities.User> Users { get; set; } = null!; // "null forgiving operator"
        public DbSet<Entities.Device> Devices { get; set; } = null!;

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options
                .UseLazyLoadingProxies()
                .UseSqlite("Data Source=test1.db");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entities.User>()
                .HasIndex(u => u.Username)
                .IsUnique();
        }
    }
}
