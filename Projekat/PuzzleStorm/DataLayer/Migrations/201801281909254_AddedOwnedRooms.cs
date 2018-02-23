namespace DataLayer.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddedOwnedRooms : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Games",
                c => new
                    {
                        Id = c.Int(nullable: false),
                        PuzzleForGame_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PuzzleDatas", t => t.PuzzleForGame_Id, cascadeDelete: true)
                .ForeignKey("dbo.Rooms", t => t.Id)
                .Index(t => t.Id)
                .Index(t => t.PuzzleForGame_Id);
            
            CreateTable(
                "dbo.PuzzleDatas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NumberOfPieces = c.Int(nullable: false),
                        PicturePath = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.PieceDatas",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SeqNumber = c.Int(nullable: false),
                        PartPath = c.String(),
                        ParentPuzzle_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.PuzzleDatas", t => t.ParentPuzzle_Id, cascadeDelete: true)
                .Index(t => t.ParentPuzzle_Id);
            
            CreateTable(
                "dbo.Rooms",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NumberOfRounds = c.Int(nullable: false),
                        MaxPlayers = c.Int(nullable: false),
                        Difficulty = c.Int(nullable: false),
                        IsPublic = c.Boolean(nullable: false),
                        Password = c.String(),
                        State = c.Int(nullable: false),
                        Owner_Id = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Players", t => t.Owner_Id, cascadeDelete: true)
                .Index(t => t.Owner_Id);
            
            CreateTable(
                "dbo.Players",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Username = c.String(),
                        Email = c.String(),
                        Password = c.String(),
                        IsLogged = c.Boolean(nullable: false),
                        AuthToken = c.String(),
                        Score = c.Int(nullable: false),
                        IsReady = c.Boolean(nullable: false),
                        CurrentRoom_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Rooms", t => t.CurrentRoom_Id)
                .Index(t => t.CurrentRoom_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Games", "Id", "dbo.Rooms");
            DropForeignKey("dbo.Players", "CurrentRoom_Id", "dbo.Rooms");
            DropForeignKey("dbo.Rooms", "Owner_Id", "dbo.Players");
            DropForeignKey("dbo.PieceDatas", "ParentPuzzle_Id", "dbo.PuzzleDatas");
            DropForeignKey("dbo.Games", "PuzzleForGame_Id", "dbo.PuzzleDatas");
            DropIndex("dbo.Players", new[] { "CurrentRoom_Id" });
            DropIndex("dbo.Rooms", new[] { "Owner_Id" });
            DropIndex("dbo.PieceDatas", new[] { "ParentPuzzle_Id" });
            DropIndex("dbo.Games", new[] { "PuzzleForGame_Id" });
            DropIndex("dbo.Games", new[] { "Id" });
            DropTable("dbo.Players");
            DropTable("dbo.Rooms");
            DropTable("dbo.PieceDatas");
            DropTable("dbo.PuzzleDatas");
            DropTable("dbo.Games");
        }
    }
}
