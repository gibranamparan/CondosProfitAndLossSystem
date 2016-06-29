namespace Sunvalley_PLSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class reportedMovement_listo : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.ReportedMovements", "Description", c => c.String());
            AddColumn("dbo.ReportedMovements", "service", c => c.String());
            DropColumn("dbo.ReportedMovements", "Destription");
        }
        
        public override void Down()
        {
            AddColumn("dbo.ReportedMovements", "Destription", c => c.String());
            DropColumn("dbo.ReportedMovements", "service");
            DropColumn("dbo.ReportedMovements", "Description");
        }
    }
}
