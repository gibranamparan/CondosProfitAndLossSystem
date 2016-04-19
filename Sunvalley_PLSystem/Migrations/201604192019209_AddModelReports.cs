namespace Sunvalley_PLSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddModelReports : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.AccountStatusReports",
                c => new
                    {
                        dateMonth = c.DateTime(nullable: false),
                        houseID = c.Int(nullable: false),
                        UserID = c.String(),
                        ApplicationUser_Id = c.String(maxLength: 128),
                    })
                .PrimaryKey(t => t.dateMonth)
                .ForeignKey("dbo.AspNetUsers", t => t.ApplicationUser_Id)
                .ForeignKey("dbo.Houses", t => t.houseID, cascadeDelete: true)
                .Index(t => t.houseID)
                .Index(t => t.ApplicationUser_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.AccountStatusReports", "houseID", "dbo.Houses");
            DropForeignKey("dbo.AccountStatusReports", "ApplicationUser_Id", "dbo.AspNetUsers");
            DropIndex("dbo.AccountStatusReports", new[] { "ApplicationUser_Id" });
            DropIndex("dbo.AccountStatusReports", new[] { "houseID" });
            DropTable("dbo.AccountStatusReports");
        }
    }
}
