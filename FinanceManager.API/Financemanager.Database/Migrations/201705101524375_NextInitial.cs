namespace Financemanager.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class NextInitial : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.User", "Account_ID", "dbo.Account");
            DropForeignKey("dbo.UserAccount", "Account_ID", "dbo.Account");
            DropForeignKey("dbo.UserAccount", "User_ID", "dbo.User");
            DropIndex("dbo.User", new[] { "Account_ID" });
            DropIndex("dbo.UserAccount", new[] { "Account_ID" });
            DropIndex("dbo.UserAccount", new[] { "User_ID" });
            RenameColumn(table: "dbo.UserAccount", name: "Account_ID", newName: "AccountID");
            RenameColumn(table: "dbo.UserAccount", name: "User_ID", newName: "UserID");
            AlterColumn("dbo.UserAccount", "AccountID", c => c.Int(nullable: false));
            AlterColumn("dbo.UserAccount", "UserID", c => c.Int(nullable: false));
            CreateIndex("dbo.UserAccount", "UserID");
            CreateIndex("dbo.UserAccount", "AccountID");
            AddForeignKey("dbo.UserAccount", "AccountID", "dbo.Account", "ID", cascadeDelete: true);
            AddForeignKey("dbo.UserAccount", "UserID", "dbo.User", "ID", cascadeDelete: true);
            DropColumn("dbo.User", "Account_ID");
        }
        
        public override void Down()
        {
            AddColumn("dbo.User", "Account_ID", c => c.Int());
            DropForeignKey("dbo.UserAccount", "UserID", "dbo.User");
            DropForeignKey("dbo.UserAccount", "AccountID", "dbo.Account");
            DropIndex("dbo.UserAccount", new[] { "AccountID" });
            DropIndex("dbo.UserAccount", new[] { "UserID" });
            AlterColumn("dbo.UserAccount", "UserID", c => c.Int());
            AlterColumn("dbo.UserAccount", "AccountID", c => c.Int());
            RenameColumn(table: "dbo.UserAccount", name: "UserID", newName: "User_ID");
            RenameColumn(table: "dbo.UserAccount", name: "AccountID", newName: "Account_ID");
            CreateIndex("dbo.UserAccount", "User_ID");
            CreateIndex("dbo.UserAccount", "Account_ID");
            CreateIndex("dbo.User", "Account_ID");
            AddForeignKey("dbo.UserAccount", "User_ID", "dbo.User", "ID");
            AddForeignKey("dbo.UserAccount", "Account_ID", "dbo.Account", "ID");
            AddForeignKey("dbo.User", "Account_ID", "dbo.Account", "ID");
        }
    }
}
