using Data.Migrations;
using Domain;
using System.Data.Entity;

namespace Data
{
    public class ProductContext : DbContext
    {
        public ProductContext() : base("Data Source=(local)\\SQLexpress;Initial Catalog=Pamano;Integrated Security=True")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ProductContext, Configuration>());
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Order> Orders { get; set; }
    }
}