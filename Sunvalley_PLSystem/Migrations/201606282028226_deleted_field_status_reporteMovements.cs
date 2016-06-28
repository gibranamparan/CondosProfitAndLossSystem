namespace Sunvalley_PLSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class deleted_field_status_reporteMovements : DbMigration
    {
        public override void Up()
        {
            DropColumn("dbo.ReportedMovements", "Status");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ReportedMovements", "Status", c => c.Boolean(nullable: false));
        }
    }
}
