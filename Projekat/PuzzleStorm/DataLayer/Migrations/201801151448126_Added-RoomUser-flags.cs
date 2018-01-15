namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedRoomUserflags : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.Rooms", "IsDeleted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Rooms", "IsStarted", c => c.Boolean(nullable: false));
            AddColumn("dbo.Users", "IsLogged", c => c.Boolean(nullable: false));
            AddColumn("dbo.Users", "AuthToken", c => c.String());
        }
        
        public override void Down()
        {
            DropColumn("dbo.Users", "AuthToken");
            DropColumn("dbo.Users", "IsLogged");
            DropColumn("dbo.Rooms", "IsStarted");
            DropColumn("dbo.Rooms", "IsDeleted");
        }
    }
}
