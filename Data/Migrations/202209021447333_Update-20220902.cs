namespace Data.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class Update20220902 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Users", "ChatId", c => c.Long(nullable: false));
            AlterColumn("dbo.Users", "UserName", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Users", "UserName", c => c.String(nullable: false));
            DropColumn("dbo.Users", "ChatId");
        }
    }
}
