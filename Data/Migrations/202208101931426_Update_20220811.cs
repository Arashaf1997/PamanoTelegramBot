namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update_20220811 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Products", "Size", c => c.String());
            AddColumn("dbo.Products", "Colors", c => c.String());
            AddColumn("dbo.Users", "FullName", c => c.String());
            AddColumn("dbo.Users", "PhoneNumber", c => c.String());
            AddColumn("dbo.Users", "Address", c => c.String());
            DropColumn("dbo.Users", "Password");
            DropColumn("dbo.Users", "EmailAddress");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Users", "EmailAddress", c => c.String());
            AddColumn("dbo.Users", "Password", c => c.String(nullable: false));
            DropColumn("dbo.Users", "Address");
            DropColumn("dbo.Users", "PhoneNumber");
            DropColumn("dbo.Users", "FullName");
            DropColumn("dbo.Products", "Colors");
            DropColumn("dbo.Products", "Size");
        }
    }
}
