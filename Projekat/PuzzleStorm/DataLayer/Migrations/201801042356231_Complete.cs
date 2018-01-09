using System.Data.Entity.Migrations;

namespace DataLayer.Migrations
{
    public partial class Complete : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Games",
                c => new
                    {
                        Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Rooms", t => t.Id)
                .ForeignKey("dbo.Puzzles", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.Players",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        Score = c.Int(nullable: false),
                        IsReady = c.Boolean(nullable: false),
                        CurrentRoom_Id = c.Int(),
                        CurrentGame_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Rooms", t => t.CurrentRoom_Id)
                .ForeignKey("dbo.Users", t => t.Id)
                .ForeignKey("dbo.Games", t => t.CurrentGame_Id)
                .Index(t => t.Id)
                .Index(t => t.CurrentRoom_Id)
                .Index(t => t.CurrentGame_Id);
            
            CreateTable(
                "dbo.Rooms",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        IsPublic = c.Boolean(nullable: false),
                        Password = c.String(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.RoomProperties", t => t.Id)
                .Index(t => t.Id);
            
            CreateTable(
                "dbo.Puzzles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NumberOfPieces = c.Int(nullable: false),
                        PicturePath = c.String(),
                        RoomOfPuzzle_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Rooms", t => t.RoomOfPuzzle_Id)
                .Index(t => t.RoomOfPuzzle_Id);
            
            CreateTable(
                "dbo.Pieces",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SeqNumber = c.Int(nullable: false),
                        PartPath = c.String(),
                        State = c.Boolean(nullable: false),
                        ParentPuzzle_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Puzzles", t => t.ParentPuzzle_Id, cascadeDelete: true)
                .Index(t => t.ParentPuzzle_Id);
            
            CreateTable(
                "dbo.RoomProperties",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NumberOfRounds = c.Int(nullable: false),
                        MaxPlayers = c.Int(nullable: false),
                        Level = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Username = c.String(nullable: false),
                        Email = c.String(),
                        Password = c.String(nullable: false),
                    })
                .PrimaryKey(t => t.Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Players", "CurrentGame_Id", "dbo.Games");
            DropForeignKey("dbo.Players", "Id", "dbo.Users");
            DropForeignKey("dbo.Rooms", "Id", "dbo.RoomProperties");
            DropForeignKey("dbo.Puzzles", "RoomOfPuzzle_Id", "dbo.Rooms");
            DropForeignKey("dbo.Pieces", "ParentPuzzle_Id", "dbo.Puzzles");
            DropForeignKey("dbo.Games", "Id", "dbo.Puzzles");
            DropForeignKey("dbo.Players", "CurrentRoom_Id", "dbo.Rooms");
            DropForeignKey("dbo.Games", "Id", "dbo.Rooms");
            DropIndex("dbo.Pieces", new[] { "ParentPuzzle_Id" });
            DropIndex("dbo.Puzzles", new[] { "RoomOfPuzzle_Id" });
            DropIndex("dbo.Rooms", new[] { "Id" });
            DropIndex("dbo.Players", new[] { "CurrentGame_Id" });
            DropIndex("dbo.Players", new[] { "CurrentRoom_Id" });
            DropIndex("dbo.Players", new[] { "Id" });
            DropIndex("dbo.Games", new[] { "Id" });
            DropTable("dbo.Users");
            DropTable("dbo.RoomProperties");
            DropTable("dbo.Pieces");
            DropTable("dbo.Puzzles");
            DropTable("dbo.Rooms");
            DropTable("dbo.Players");
            DropTable("dbo.Games");
        }
    }
}
