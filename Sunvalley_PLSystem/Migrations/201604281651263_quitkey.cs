namespace Sunvalley_PLSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class quitkey : DbMigration
    {
        public override void Up()
        {
            DropPrimaryKey("dbo.AccountStatusReports");
            AddColumn("dbo.AccountStatusReports", "accountStatusReportID", c => c.Int(nullable: false, identity: true));
            AddPrimaryKey("dbo.AccountStatusReports", "accountStatusReportID");
        }
        
        public override void Down()
        {
            DropPrimaryKey("dbo.AccountStatusReports");
            DropColumn("dbo.AccountStatusReports", "accountStatusReportID");
            AddPrimaryKey("dbo.AccountStatusReports", "dateMonth");
        }
    }
}
