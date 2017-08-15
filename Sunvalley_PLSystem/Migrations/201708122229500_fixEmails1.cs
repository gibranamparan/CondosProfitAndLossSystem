namespace Sunvalley_PLSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class fixEmails1 : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.AspNetUsers", "otrosEmail", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.AspNetUsers", "otrosEmail");
        }
    }
}
