namespace Sunvalley_PLSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class addNewAtribute : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Movements", "state", c => c.Boolean(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.Movements", "state");
        }
    }
}
