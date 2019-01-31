namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BatchNumberInventoryTransactions : DbMigration
    {
        public override void Up()
        {
            AddColumn("INVENTORY.GoodsIssues", "BatchNumber", c => c.String(maxLength: 100, unicode: false));
            AddColumn("INVENTORY.GoodsIssues", "BatchDate", c => c.DateTime(nullable: false));
            AddColumn("INVENTORY.GoodsReceipts", "BatchNumber", c => c.String(maxLength: 100, unicode: false));
            AddColumn("INVENTORY.GoodsReceipts", "BatchDate", c => c.DateTime(nullable: false));
            AddColumn("INVENTORY.GoodsReturns", "BatchNumber", c => c.String(maxLength: 100, unicode: false));
            AddColumn("INVENTORY.GoodsReturns", "BatchDate", c => c.DateTime(nullable: false));
        }
        
        public override void Down()
        {
            DropColumn("INVENTORY.GoodsReturns", "BatchDate");
            DropColumn("INVENTORY.GoodsReturns", "BatchNumber");
            DropColumn("INVENTORY.GoodsReceipts", "BatchDate");
            DropColumn("INVENTORY.GoodsReceipts", "BatchNumber");
            DropColumn("INVENTORY.GoodsIssues", "BatchDate");
            DropColumn("INVENTORY.GoodsIssues", "BatchNumber");
        }
    }
}
