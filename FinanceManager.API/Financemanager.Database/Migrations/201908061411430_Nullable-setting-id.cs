namespace FinanceManager.Database.Migrations
{
    using System.Data.Entity.Migrations;
    
    public partial class Nullablesettingid : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("dbo.MoneyOperation", "OperationSettingID", "dbo.MoneyOperationSetting");
            DropIndex("dbo.MoneyOperation", new[] { "OperationSettingID" });
            AlterColumn("dbo.MoneyOperation", "OperationSettingID", c => c.Int());
            CreateIndex("dbo.MoneyOperation", "OperationSettingID");
            AddForeignKey("dbo.MoneyOperation", "OperationSettingID", "dbo.MoneyOperationSetting", "ID");
        }
        
        public override void Down()
        {
            DropForeignKey("dbo.MoneyOperation", "OperationSettingID", "dbo.MoneyOperationSetting");
            DropIndex("dbo.MoneyOperation", new[] { "OperationSettingID" });
            AlterColumn("dbo.MoneyOperation", "OperationSettingID", c => c.Int(nullable: false));
            CreateIndex("dbo.MoneyOperation", "OperationSettingID");
            AddForeignKey("dbo.MoneyOperation", "OperationSettingID", "dbo.MoneyOperationSetting", "ID", cascadeDelete: true);
        }
    }
}
