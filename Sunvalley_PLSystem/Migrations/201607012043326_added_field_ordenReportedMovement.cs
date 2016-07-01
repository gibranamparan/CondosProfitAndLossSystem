namespace Sunvalley_PLSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_field_ordenReportedMovement : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ReportedMovements", "orden", c => c.Int(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("dbo.ReportedMovements", "orden");
        }
    }
}
