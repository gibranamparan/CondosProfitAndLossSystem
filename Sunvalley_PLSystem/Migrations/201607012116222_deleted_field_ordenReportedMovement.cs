namespace Sunvalley_PLSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deleted_field_ordenReportedMovement : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ReportedMovements", "orden");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ReportedMovements", "orden", c => c.Int(nullable: false));
        }
    }
}
