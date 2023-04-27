using Microsoft.EntityFrameworkCore;

namespace SoftwareCatalogDatabaseASP.Models
{
    public class SoftwareCatalogDBContext : DbContext
    {
        public SoftwareCatalogDBContext(DbContextOptions<SoftwareCatalogDBContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
        public DbSet<Software> Softwares { get; set; }
        public DbSet<Screens> Screens { get; set; }
        public DbSet<Comments> Comments { get; set; }
        public DbSet<Categories> Categories { get; set; }

    }
}
