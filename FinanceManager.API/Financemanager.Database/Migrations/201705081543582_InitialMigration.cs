namespace Financemanager.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class InitialMigration : DbMigration
    {
        public override void Up()
        {
            CreateTable(
                "dbo.Account",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        IsActive = c.Boolean(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        InitialAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CurrentAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        CreationDate = c.DateTime(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.User",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        IsActive = c.Boolean(nullable: false),
                        UserName = c.String(),
                        FirstName = c.String(),
                        LastName = c.String(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.PeriodicIncome",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        IsReal = c.Boolean(nullable: false),
                        IsActive = c.Boolean(nullable: false),
                        ValidityBeginDate = c.DateTime(nullable: false),
                        ValidityEndDate = c.DateTime(nullable: false),
                        RepetitionUnitQuantity = c.Short(nullable: false),
                        RepetitionUnit = c.Int(nullable: false),
                        InitialAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Description = c.String(),
                        Account_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Account", t => t.Account_ID)
                .Index(t => t.Account_ID);
            
            CreateTable(
                "dbo.SingleIncome",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        IsActive = c.Boolean(nullable: false),
                        Name = c.String(),
                        Description = c.String(),
                        Date = c.DateTime(nullable: false),
                        Amount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        IsAlreadyProcessed = c.Boolean(nullable: false),
                        IsReal = c.Boolean(nullable: false),
                        Account_ID = c.Int(),
                        Category_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Account", t => t.Account_ID)
                .ForeignKey("dbo.IncomeCategory", t => t.Category_ID)
                .Index(t => t.Account_ID)
                .Index(t => t.Category_ID);
            
            CreateTable(
                "dbo.IncomeCategory",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        IsActive = c.Boolean(nullable: false),
                        Name = c.String(),
                        SubCategory_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.IncomeCategory", t => t.SubCategory_ID)
                .Index(t => t.SubCategory_ID);
            
            CreateTable(
                "dbo.UsersAccounts",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        IsActive = c.Boolean(nullable: false),
                        Account_ID = c.Int(),
                        User_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Account", t => t.Account_ID)
                .ForeignKey("dbo.User", t => t.User_ID)
                .Index(t => t.Account_ID)
                .Index(t => t.User_ID);
            
            CreateTable(
                "dbo.UserAccount",
                c => new
                    {
                        User_ID = c.Int(nullable: false),
                        Account_ID = c.Int(nullable: false),
                    })
                .PrimaryKey(t => new { t.User_ID, t.Account_ID })
                .ForeignKey("dbo.User", t => t.User_ID, cascadeDelete: true)
                .ForeignKey("dbo.Account", t => t.Account_ID, cascadeDelete: true)
                .Index(t => t.User_ID)
                .Index(t => t.Account_ID);
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.UsersAccounts", "User_ID", "dbo.User");
            DropForeignKey("dbo.UsersAccounts", "Account_ID", "dbo.Account");
            DropForeignKey("dbo.SingleIncome", "Category_ID", "dbo.IncomeCategory");
            DropForeignKey("dbo.IncomeCategory", "SubCategory_ID", "dbo.IncomeCategory");
            DropForeignKey("dbo.SingleIncome", "Account_ID", "dbo.Account");
            DropForeignKey("dbo.PeriodicIncome", "Account_ID", "dbo.Account");
            DropForeignKey("dbo.UserAccount", "Account_ID", "dbo.Account");
            DropForeignKey("dbo.UserAccount", "User_ID", "dbo.User");
            DropIndex("dbo.UserAccount", new[] { "Account_ID" });
            DropIndex("dbo.UserAccount", new[] { "User_ID" });
            DropIndex("dbo.UsersAccounts", new[] { "User_ID" });
            DropIndex("dbo.UsersAccounts", new[] { "Account_ID" });
            DropIndex("dbo.IncomeCategory", new[] { "SubCategory_ID" });
            DropIndex("dbo.SingleIncome", new[] { "Category_ID" });
            DropIndex("dbo.SingleIncome", new[] { "Account_ID" });
            DropIndex("dbo.PeriodicIncome", new[] { "Account_ID" });
            DropTable("dbo.UserAccount");
            DropTable("dbo.UsersAccounts");
            DropTable("dbo.IncomeCategory");
            DropTable("dbo.SingleIncome");
            DropTable("dbo.PeriodicIncome");
            DropTable("dbo.User");
            DropTable("dbo.Account");
        }
    }
}
