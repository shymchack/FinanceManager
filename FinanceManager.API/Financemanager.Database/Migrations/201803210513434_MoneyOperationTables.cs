namespace FinanceManager.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class MoneyOperationTables : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MoneyOperation", "MoneyOperationSetting_ID", "dbo.MoneyOperationSetting");
            DropIndex("dbo.MoneyOperation", new[] { "MoneyOperationSetting_ID" });
            DropColumn("dbo.MoneyOperation", "OperationSettingID");
            RenameColumn(table: "dbo.MoneyOperation", name: "Account_ID", newName: "AccountID");
            RenameColumn(table: "dbo.MoneyOperation", name: "MoneyOperationSetting_ID", newName: "OperationSettingID");
            RenameIndex(table: "dbo.MoneyOperation", name: "IX_Account_ID", newName: "IX_AccountID");
            CreateTable(
                "dbo.MoneyOperationChange",
                c => new
                    {
                        ID = c.Int(nullable: false, identity: true),
                        ChangeDate = c.DateTime(nullable: false),
                        MoneyOperationID = c.Int(nullable: false),
                        ChangeAmount = c.Decimal(nullable: false, precision: 18, scale: 2),
                    })
                .PrimaryKey(t => t.ID)
                .ForeignKey("dbo.MoneyOperation", t => t.MoneyOperationID, cascadeDelete: true)
                .Index(t => t.MoneyOperationID);
            
            AlterColumn("dbo.MoneyOperation", "OperationSettingID", c => c.Int(nullable: false));
            CreateIndex("dbo.MoneyOperation", "OperationSettingID");
            AddForeignKey("dbo.MoneyOperation", "OperationSettingID", "dbo.MoneyOperationSetting", "ID", cascadeDelete: true);
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MoneyOperation", "OperationSettingID", "dbo.MoneyOperationSetting");
            DropForeignKey("dbo.MoneyOperationChange", "MoneyOperationID", "dbo.MoneyOperation");
            DropIndex("dbo.MoneyOperationChange", new[] { "MoneyOperationID" });
            DropIndex("dbo.MoneyOperation", new[] { "OperationSettingID" });
            AlterColumn("dbo.MoneyOperation", "OperationSettingID", c => c.Int());
            DropTable("dbo.MoneyOperationChange");
            RenameIndex(table: "dbo.MoneyOperation", name: "IX_AccountID", newName: "IX_Account_ID");
            RenameColumn(table: "dbo.MoneyOperation", name: "OperationSettingID", newName: "MoneyOperationSetting_ID");
            RenameColumn(table: "dbo.MoneyOperation", name: "AccountID", newName: "Account_ID");
            AddColumn("dbo.MoneyOperation", "OperationSettingID", c => c.Int(nullable: false));
            CreateIndex("dbo.MoneyOperation", "MoneyOperationSetting_ID");
            AddForeignKey("dbo.MoneyOperation", "MoneyOperationSetting_ID", "dbo.MoneyOperationSetting", "ID");
        }
    }
}
