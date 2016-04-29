namespace Sunvalley_PLSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class house_changedAttr_postalCodeToString : DbMigration
    {
        public override void Up()
        {
            AlterColumn("dbo.Houses", "postalCode", c => c.String());
        }
        
        public override void Down()
        {
            AlterColumn("dbo.Houses", "postalCode", c => c.Int(nullable: false));
        }
    }
}
