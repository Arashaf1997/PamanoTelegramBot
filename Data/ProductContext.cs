using Data.Migrations;
using Domain;
using System.Data.Entity;

namespace Data
{
    public class ProductContext : DbContext
    {
        public ProductContext() : base("Data Source=SQL8001.site4now.net;Initial Catalog=db_a8c873_pamanobot;User Id=db_a8c873_pamanobot_admin;Password=kNjJMJ6uiSEMkbX")
        {
            Database.SetInitializer(new MigrateDatabaseToLatestVersion<ProductContext, Configuration>());
        }
        public DbSet<Product> Products { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<ProductImage> ProductImages { get; set; }
        public DbSet<Order> Orders { get; set; }
    }
}