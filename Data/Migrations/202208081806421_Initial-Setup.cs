namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialSetup : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Products", "EditTime", c => c.DateTime());
            AlterColumn("dbo.Users", "EditTime", c => c.DateTime());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Users", "EditTime", c => c.DateTime(nullable: false));
            AlterColumn("dbo.Products", "EditTime", c => c.DateTime(nullable: false));
        }
    }
}
