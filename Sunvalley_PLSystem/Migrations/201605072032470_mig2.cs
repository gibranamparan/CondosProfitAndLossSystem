namespace Sunvalley_PLSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class mig2 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.GeneralInformations",
                c => new
                    {
                        InfoID = c.Int(nullable: false, identity: true),
                        InformacionGen = c.String(),
                    })
                .PrimaryKey(t => t.InfoID);
            
        }
        
        public override void Down()
        {
            DropTable("dbo.GeneralInformations");
        }
    }
}
