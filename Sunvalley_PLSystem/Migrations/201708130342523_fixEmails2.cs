namespace Sunvalley_PLSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixEmails2 : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.AspNetUsers", "Email1");
            DropColumn("dbo.AspNetUsers", "Email2");
        }
        
        public override void Down()
        {
            AddColumn("dbo.AspNetUsers", "Email2", c => c.String());
            AddColumn("dbo.AspNetUsers", "Email1", c => c.String());
        }
    }
}
