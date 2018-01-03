namespace Server.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialModel : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Games",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        CurrentPointer_Id = c.Int(),
                        Puzzle_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Players", t => t.CurrentPointer_Id)
                .ForeignKey("dbo.Puzzles", t => t.Puzzle_Id)
                .Index(t => t.CurrentPointer_Id)
                .Index(t => t.Puzzle_Id);
            
            CreateTable(
                "dbo.Players",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Score = c.Int(nullable: false),
                        User_Id = c.Int(),
                        Game_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Users", t => t.User_Id)
                .ForeignKey("dbo.Games", t => t.Game_Id)
                .Index(t => t.User_Id)
                .Index(t => t.Game_Id);
            
            CreateTable(
                "dbo.Users",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        Username = c.String(),
                        Password = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Puzzles",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        NumberOfPieces = c.Int(nullable: false),
                        PicturePath = c.String(),
                    })
                .PrimaryKey(t => t.Id);
            
            CreateTable(
                "dbo.Pieces",
                c => new
                    {
                        Id = c.Int(nullable: false, identity: true),
                        SeqNumber = c.Int(nullable: false),
                        PartPath = c.String(),
                        State = c.Boolean(nullable: false),
                        Puzzle_Id = c.Int(),
                    })
                .PrimaryKey(t => t.Id)
                .ForeignKey("dbo.Puzzles", t => t.Puzzle_Id)
                .Index(t => t.Puzzle_Id);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.Games", "Puzzle_Id", "dbo.Puzzles");
            DropForeignKey("dbo.Pieces", "Puzzle_Id", "dbo.Puzzles");
            DropForeignKey("dbo.Players", "Game_Id", "dbo.Games");
            DropForeignKey("dbo.Games", "CurrentPointer_Id", "dbo.Players");
            DropForeignKey("dbo.Players", "User_Id", "dbo.Users");
            DropIndex("dbo.Pieces", new[] { "Puzzle_Id" });
            DropIndex("dbo.Players", new[] { "Game_Id" });
            DropIndex("dbo.Players", new[] { "User_Id" });
            DropIndex("dbo.Games", new[] { "Puzzle_Id" });
            DropIndex("dbo.Games", new[] { "CurrentPointer_Id" });
            DropTable("dbo.Pieces");
            DropTable("dbo.Puzzles");
            DropTable("dbo.Users");
            DropTable("dbo.Players");
            DropTable("dbo.Games");
        }
    }
}
