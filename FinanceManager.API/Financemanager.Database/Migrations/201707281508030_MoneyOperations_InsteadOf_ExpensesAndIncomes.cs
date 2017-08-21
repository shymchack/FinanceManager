namespace FinanceManager.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MoneyOperations_InsteadOf_ExpensesAndIncomes : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.PeriodicExpense", "OperationSettingID", "dbo.OperationSetting");
            RenameTable(name: "dbo.PeriodicExpense", newName: "MoneyOperation");
            DropForeignKey("dbo.PeriodicIncome", "OperationSettingID", "dbo.OperationSetting");
            DropForeignKey("dbo.SingleExpense", "Account_ID", "dbo.Account");
            DropForeignKey("dbo.ExpenseCategory", "SubCategory_ID", "dbo.ExpenseCategory");
            DropForeignKey("dbo.SingleExpense", "Category_ID", "dbo.ExpenseCategory");
            DropForeignKey("dbo.SingleExpense", "OperationSettingID", "dbo.OperationSetting");
            DropForeignKey("dbo.SingleIncome", "Account_ID", "dbo.Account");
            DropForeignKey("dbo.IncomeCategory", "SubCategory_ID", "dbo.IncomeCategory");
            DropForeignKey("dbo.SingleIncome", "Category_ID", "dbo.IncomeCategory");
            DropForeignKey("dbo.SingleIncome", "OperationSettingID", "dbo.OperationSetting");
            DropIndex("dbo.MoneyOperation", new[] { "OperationSettingID" });
            DropIndex("dbo.PeriodicIncome", new[] { "OperationSettingID" });
            DropIndex("dbo.PeriodicIncome", new[] { "Account_ID" });
            DropIndex("dbo.SingleExpense", new[] { "OperationSettingID" });
            DropIndex("dbo.SingleExpense", new[] { "Account_ID" });
            DropIndex("dbo.SingleExpense", new[] { "Category_ID" });
            DropIndex("dbo.ExpenseCategory", new[] { "SubCategory_ID" });
            DropIndex("dbo.SingleIncome", new[] { "OperationSettingID" });
            DropIndex("dbo.SingleIncome", new[] { "Account_ID" });
            DropIndex("dbo.SingleIncome", new[] { "Category_ID" });
            DropIndex("dbo.IncomeCategory", new[] { "SubCategory_ID" });
            CreateTable(
                "dbo.MoneyOperationSetting",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ReservePeriodUnit = c.Int(nullable: false),
                        ReservePeriodQuantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            AddColumn("dbo.MoneyOperation", "Name", c => c.String());
            AddColumn("dbo.MoneyOperation", "MoneyOperationSetting_ID", c => c.Int());
            CreateIndex("dbo.MoneyOperation", "MoneyOperationSetting_ID");
            AddForeignKey("dbo.MoneyOperation", "MoneyOperationSetting_ID", "dbo.MoneyOperationSetting", "ID");
            DropTable("dbo.OperationSetting");
            DropTable("dbo.PeriodicIncome");
            DropTable("dbo.SingleExpense");
            DropTable("dbo.ExpenseCategory");
            DropTable("dbo.SingleIncome");
            DropTable("dbo.IncomeCategory");
        }
        
        public override void Down()
        {
            CreateTable(
                "dbo.IncomeCategory",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        IsActive = c.Boolean(nullable: false),
                        Name = c.String(),
                        SubCategory_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID);
            
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
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.ExpenseCategory",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        IsActive = c.Boolean(nullable: false),
                        Name = c.String(),
                        SubCategory_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID);
            
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
                .PrimaryKey(t => t.ID);
            
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
                        NextOperationExecutionDate = c.DateTime(nullable: false),
                        RepetitionUnitQuantity = c.Short(nullable: false),
                        RepetitionUnit = c.Int(nullable: false),
                        InitialAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                        Description = c.String(),
                        Account_ID = c.Int(),
                    })
                .PrimaryKey(t => t.ID);
            
            CreateTable(
                "dbo.OperationSetting",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ReservePeriodUnit = c.Int(nullable: false),
                        ReservePeriodQuantity = c.Int(nullable: false),
                    })
                .PrimaryKey(t => t.ID);
            
            DropForeignKey("dbo.MoneyOperation", "MoneyOperationSetting_ID", "dbo.MoneyOperationSetting");
            DropIndex("dbo.MoneyOperation", new[] { "MoneyOperationSetting_ID" });
            DropColumn("dbo.MoneyOperation", "MoneyOperationSetting_ID");
            DropColumn("dbo.MoneyOperation", "Name");
            DropTable("dbo.MoneyOperationSetting");
            CreateIndex("dbo.IncomeCategory", "SubCategory_ID");
            CreateIndex("dbo.SingleIncome", "Category_ID");
            CreateIndex("dbo.SingleIncome", "Account_ID");
            CreateIndex("dbo.SingleIncome", "OperationSettingID");
            CreateIndex("dbo.ExpenseCategory", "SubCategory_ID");
            CreateIndex("dbo.SingleExpense", "Category_ID");
            CreateIndex("dbo.SingleExpense", "Account_ID");
            CreateIndex("dbo.SingleExpense", "OperationSettingID");
            CreateIndex("dbo.PeriodicIncome", "Account_ID");
            CreateIndex("dbo.PeriodicIncome", "OperationSettingID");
            CreateIndex("dbo.MoneyOperation", "OperationSettingID");
            AddForeignKey("dbo.SingleIncome", "OperationSettingID", "dbo.OperationSetting", "ID", cascadeDelete: true);
            AddForeignKey("dbo.SingleIncome", "Category_ID", "dbo.IncomeCategory", "ID");
            AddForeignKey("dbo.IncomeCategory", "SubCategory_ID", "dbo.IncomeCategory", "ID");
            AddForeignKey("dbo.SingleIncome", "Account_ID", "dbo.Account", "ID");
            AddForeignKey("dbo.SingleExpense", "OperationSettingID", "dbo.OperationSetting", "ID", cascadeDelete: true);
            AddForeignKey("dbo.SingleExpense", "Category_ID", "dbo.ExpenseCategory", "ID");
            AddForeignKey("dbo.ExpenseCategory", "SubCategory_ID", "dbo.ExpenseCategory", "ID");
            AddForeignKey("dbo.SingleExpense", "Account_ID", "dbo.Account", "ID");
            AddForeignKey("dbo.PeriodicIncome", "OperationSettingID", "dbo.OperationSetting", "ID", cascadeDelete: true);
            RenameTable(name: "dbo.MoneyOperation", newName: "PeriodicExpense");
            AddForeignKey("dbo.PeriodicExpense", "OperationSettingID", "dbo.OperationSetting", "ID", cascadeDelete: true);
        }
    }
}
