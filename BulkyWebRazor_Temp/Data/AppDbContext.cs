using BulkyWebRazor_Temp.Models;
using Microsoft.EntityFrameworkCore;

namespace BulkyWebRazor_Temp.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions options) : base(options)
        {

        }

        public DbSet<Category> Categories { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Category>().HasData
                (
                new Category { ID = 1, Name = "Action", DisplayOrder = 1 },
                new Category { ID = 2, Name = "SciFic", DisplayOrder = 2 },
                new Category { ID = 3, Name = "History", DisplayOrder = 3 }
                );
        }

    }
}
