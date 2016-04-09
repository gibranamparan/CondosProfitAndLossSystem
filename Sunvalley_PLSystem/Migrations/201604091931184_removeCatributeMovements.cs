namespace Sunvalley_PLSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class removeCatributeMovements : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.Movements", "value");
            DropColumn("dbo.Movements", "qty");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Movements", "qty", c => c.Int(nullable: false));
            AddColumn("dbo.Movements", "value", c => c.Decimal(nullable: false, precision: 18, scale: 2));
        }
    }
}
