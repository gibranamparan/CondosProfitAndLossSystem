namespace Sunvalley_PLSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class added_table_ReportedMovements : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.ReportedMovements",
                c => new
                    {
                        MovementID = c.Int(nullable: false, identity: true),
                        RegisterBy = c.String(),
                        TransactionDate = c.DateTime(nullable: false),
                        Type = c.String(),
                        Destription = c.String(),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Balance = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Status = c.Boolean(nullable: false),
                        accountStatusReportID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.MovementID)
                .ForeignKey("dbo.AccountStatusReports", t => t.accountStatusReportID, cascadeDelete: true)
                .Index(t => t.accountStatusReportID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.ReportedMovements", "accountStatusReportID", "dbo.AccountStatusReports");
            DropIndex("dbo.ReportedMovements", new[] { "accountStatusReportID" });
            DropTable("dbo.ReportedMovements");
        }
    }
}
