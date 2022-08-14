namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_20220814 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Orders",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        TotalPrice = c.Int(nullable: false),
                        TotalCount = c.Int(nullable: false),
                        Details = c.String(),
                        CustomerDescription = c.String(),
                        UserId = c.Int(),
                        ProductId = c.Int(),
                        InsertTime = c.DateTime(nullable: false),
                        EditTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Products", t => t.ProductId)
                .ForeignKey("dbo.Users", t => t.UserId)
                .Index(t => t.UserId)
                .Index(t => t.ProductId);
            
            AddColumn("dbo.Products", "SeriesCount", c => c.Int(nullable: false));
            AddColumn("dbo.Users", "StoreName", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Orders", "UserId", "dbo.Users");
            DropForeignKey("dbo.Orders", "ProductId", "dbo.Products");
            DropIndex("dbo.Orders", new[] { "ProductId" });
            DropIndex("dbo.Orders", new[] { "UserId" });
            DropColumn("dbo.Users", "StoreName");
            DropColumn("dbo.Products", "SeriesCount");
            DropTable("dbo.Orders");
        }
    }
}
