namespace FinanceManager.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddAuthenticationToken : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.UserToken",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        UserID = c.Int(nullable: false),
                        TokenID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Token", t => t.TokenID, cascadeDelete: true)
                .ForeignKey("dbo.User", t => t.UserID, cascadeDelete: true)
                .Index(t => t.UserID)
                .Index(t => t.TokenID);
            
            CreateTable(
                "dbo.Token",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        TokenData = c.String(),
                        ExpirationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.User", "PassHash", c => c.String());
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UserToken", "UserID", "dbo.User");
            DropForeignKey("dbo.UserToken", "TokenID", "dbo.Token");
            DropIndex("dbo.UserToken", new[] { "TokenID" });
            DropIndex("dbo.UserToken", new[] { "UserID" });
            DropColumn("dbo.User", "PassHash");
            DropTable("dbo.Token");
            DropTable("dbo.UserToken");
        }
    }
}
