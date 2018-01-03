namespace Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialModel1 : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Rooms",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NumberOfRounds = c.Int(nullable: false),
                        MaxPlayers = c.Int(nullable: false),
                        Difficulty = c.Int(nullable: false),
                        IsPublic = c.Boolean(nullable: false),
                        Creator_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Players", t => t.Creator_Id)
                .Index(t => t.Creator_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Rooms", "Creator_Id", "dbo.Players");
            DropIndex("dbo.Rooms", new[] { "Creator_Id" });
            DropTable("dbo.Rooms");
        }
    }
}
