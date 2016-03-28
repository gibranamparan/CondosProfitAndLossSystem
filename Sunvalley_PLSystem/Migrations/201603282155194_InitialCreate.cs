namespace Sunvalley_PLSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialCreate : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.AspNetUsers", "postalCode", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.AspNetUsers", "postalCode", c => c.Int(nullable: false));
        }
    }
}
