namespace Sunvalley_PLSystem.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class House_cambioNombre_UserIDAId : DbMigration
    {
        public override void Up()
        {
            RenameColumn(table: "dbo.Houses", name: "ApplicationUser_Id", newName: "Id");
            RenameIndex(table: "dbo.Houses", name: "IX_ApplicationUser_Id", newName: "IX_Id");
            DropColumn("dbo.Houses", "UserID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.Houses", "UserID", c => c.String());
            RenameIndex(table: "dbo.Houses", name: "IX_Id", newName: "IX_ApplicationUser_Id");
            RenameColumn(table: "dbo.Houses", name: "Id", newName: "ApplicationUser_Id");
        }
    }
}
