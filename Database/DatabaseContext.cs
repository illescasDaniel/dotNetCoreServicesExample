using System;
using Microsoft.EntityFrameworkCore;

namespace myMicroservice.Database
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Entities.User> Users { get; set; } = null!; // "null forgiving operator"

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=test8.db");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Entities.User>()
                .HasIndex(u => u.Username)
                .IsUnique();
        }
    }
}
