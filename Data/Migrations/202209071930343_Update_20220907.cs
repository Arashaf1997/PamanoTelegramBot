namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_20220907 : DbMigration
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
            
            CreateTable(
                "dbo.Products",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Name = c.String(),
                        Price = c.Int(nullable: false),
                        SeriesCount = c.Int(nullable: false),
                        Description = c.String(),
                        Size = c.String(),
                        Colors = c.String(),
                        UserId = c.Int(nullable: false),
                        InsertTime = c.DateTime(nullable: false),
                        EditTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.UserId, cascadeDelete: true)
                .Index(t => t.UserId);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ChatId = c.Long(nullable: false),
                        UserName = c.String(),
                        FullName = c.String(),
                        StoreName = c.String(),
                        PhoneNumber = c.String(),
                        Address = c.String(),
                        InsertTime = c.DateTime(nullable: false),
                        EditTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.ProductImages",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        ProductId = c.Int(nullable: false),
                        ImageId = c.String(),
                        InsertTime = c.DateTime(nullable: false),
                        EditTime = c.DateTime(),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Orders", "UserId", "dbo.Users");
            DropForeignKey("dbo.Orders", "ProductId", "dbo.Products");
            DropForeignKey("dbo.Products", "UserId", "dbo.Users");
            DropIndex("dbo.Products", new[] { "UserId" });
            DropIndex("dbo.Orders", new[] { "ProductId" });
            DropIndex("dbo.Orders", new[] { "UserId" });
            DropTable("dbo.ProductImages");
            DropTable("dbo.Users");
            DropTable("dbo.Products");
            DropTable("dbo.Orders");
        }
    }
}
