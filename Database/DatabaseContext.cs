using System;
using Microsoft.EntityFrameworkCore;

namespace myMicroservice.Database
{
    public class DatabaseContext : DbContext
    {
        public DbSet<Entities.User> Users { get; set; } = null!; // "null forgiving operator" xD

        protected override void OnConfiguring(DbContextOptionsBuilder options)
            => options.UseSqlite("Data Source=test2.db");
    }
}
