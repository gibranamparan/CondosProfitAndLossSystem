namespace Sunvalley_PLSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class services : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Services",
                c => new
                    {
                        serviceID = c.Int(nullable: false, identity: true),
                        name = c.String(),
                    })
                .PrimaryKey(t => t.serviceID);
            
            AddColumn("dbo.Movements", "typeOfMovement", c => c.String());
            AddColumn("dbo.Movements", "serviceID", c => c.Int(nullable: false));
            CreateIndex("dbo.Movements", "serviceID");
            AddForeignKey("dbo.Movements", "serviceID", "dbo.Services", "serviceID", cascadeDelete: true);
            DropColumn("dbo.Movements", "code");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Movements", "code", c => c.String());
            DropForeignKey("dbo.Movements", "serviceID", "dbo.Services");
            DropIndex("dbo.Movements", new[] { "serviceID" });
            DropColumn("dbo.Movements", "serviceID");
            DropColumn("dbo.Movements", "typeOfMovement");
            DropTable("dbo.Services");
        }
    }
}
