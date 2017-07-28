namespace Financemanager.Database.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class AddColumn_NextOperationExecutionDate : DbMigration
    {
        public override void Up()
        {
            AddColumn("dbo.PeriodicExpense", "NextOperationExecutionDate", c => c.DateTime(nullable: true));
            AddColumn("dbo.PeriodicIncome", "NextOperationExecutionDate", c => c.DateTime(nullable: true));
        }
        
        public override void Down()
        {
            DropColumn("dbo.PeriodicIncome", "NextOperationExecutionDate");
            DropColumn("dbo.PeriodicExpense", "NextOperationExecutionDate");
        }
    }
}
