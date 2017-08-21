namespace FinanceManager.Database.Migrations
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
                        Account_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.Account", t => t.Account_ID)
                .Index(t => t.Account_ID);
            
            CreateTable(
                "dbo.UserAccount",
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
                "dbo.OperationSetting",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ReservePeriodUnit = c.Int(nullable: false),
                        ReservePeriodQuantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.PeriodicExpense",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        OperationSettingID = c.Int(nullable: false),
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
                .ForeignKey("dbo.OperationSetting", t => t.OperationSettingID, cascadeDelete: true)
                .Index(t => t.OperationSettingID)
                .Index(t => t.Account_ID);
            
            CreateTable(
                "dbo.PeriodicIncome",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        OperationSettingID = c.Int(nullable: false),
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
                .ForeignKey("dbo.OperationSetting", t => t.OperationSettingID, cascadeDelete: true)
                .Index(t => t.OperationSettingID)
                .Index(t => t.Account_ID);
            
            CreateTable(
                "dbo.SingleExpense",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        OperationSettingID = c.Int(nullable: false),
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
                .ForeignKey("dbo.ExpenseCategory", t => t.Category_ID)
                .ForeignKey("dbo.OperationSetting", t => t.OperationSettingID, cascadeDelete: true)
                .Index(t => t.OperationSettingID)
                .Index(t => t.Account_ID)
                .Index(t => t.Category_ID);
            
            CreateTable(
                "dbo.ExpenseCategory",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        IsActive = c.Boolean(nullable: false),
                        Name = c.String(),
                        SubCategory_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.ExpenseCategory", t => t.SubCategory_ID)
                .Index(t => t.SubCategory_ID);
            
            CreateTable(
                "dbo.SingleIncome",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        OperationSettingID = c.Int(nullable: false),
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
                .ForeignKey("dbo.OperationSetting", t => t.OperationSettingID, cascadeDelete: true)
                .Index(t => t.OperationSettingID)
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
            
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.SingleIncome", "OperationSettingID", "dbo.OperationSetting");
            DropForeignKey("dbo.SingleIncome", "Category_ID", "dbo.IncomeCategory");
            DropForeignKey("dbo.IncomeCategory", "SubCategory_ID", "dbo.IncomeCategory");
            DropForeignKey("dbo.SingleIncome", "Account_ID", "dbo.Account");
            DropForeignKey("dbo.SingleExpense", "OperationSettingID", "dbo.OperationSetting");
            DropForeignKey("dbo.SingleExpense", "Category_ID", "dbo.ExpenseCategory");
            DropForeignKey("dbo.ExpenseCategory", "SubCategory_ID", "dbo.ExpenseCategory");
            DropForeignKey("dbo.SingleExpense", "Account_ID", "dbo.Account");
            DropForeignKey("dbo.PeriodicIncome", "OperationSettingID", "dbo.OperationSetting");
            DropForeignKey("dbo.PeriodicIncome", "Account_ID", "dbo.Account");
            DropForeignKey("dbo.PeriodicExpense", "OperationSettingID", "dbo.OperationSetting");
            DropForeignKey("dbo.PeriodicExpense", "Account_ID", "dbo.Account");
            DropForeignKey("dbo.User", "Account_ID", "dbo.Account");
            DropForeignKey("dbo.UserAccount", "User_ID", "dbo.User");
            DropForeignKey("dbo.UserAccount", "Account_ID", "dbo.Account");
            DropIndex("dbo.IncomeCategory", new[] { "SubCategory_ID" });
            DropIndex("dbo.SingleIncome", new[] { "Category_ID" });
            DropIndex("dbo.SingleIncome", new[] { "Account_ID" });
            DropIndex("dbo.SingleIncome", new[] { "OperationSettingID" });
            DropIndex("dbo.ExpenseCategory", new[] { "SubCategory_ID" });
            DropIndex("dbo.SingleExpense", new[] { "Category_ID" });
            DropIndex("dbo.SingleExpense", new[] { "Account_ID" });
            DropIndex("dbo.SingleExpense", new[] { "OperationSettingID" });
            DropIndex("dbo.PeriodicIncome", new[] { "Account_ID" });
            DropIndex("dbo.PeriodicIncome", new[] { "OperationSettingID" });
            DropIndex("dbo.PeriodicExpense", new[] { "Account_ID" });
            DropIndex("dbo.PeriodicExpense", new[] { "OperationSettingID" });
            DropIndex("dbo.UserAccount", new[] { "User_ID" });
            DropIndex("dbo.UserAccount", new[] { "Account_ID" });
            DropIndex("dbo.User", new[] { "Account_ID" });
            DropTable("dbo.IncomeCategory");
            DropTable("dbo.SingleIncome");
            DropTable("dbo.ExpenseCategory");
            DropTable("dbo.SingleExpense");
            DropTable("dbo.PeriodicIncome");
            DropTable("dbo.PeriodicExpense");
            DropTable("dbo.OperationSetting");
            DropTable("dbo.UserAccount");
            DropTable("dbo.User");
            DropTable("dbo.Account");
        }
    }
}
