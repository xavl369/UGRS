namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class EmptyMovementsStock : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("INVENTORY.GoodsIssues", "ItemId", "INVENTORY.Items");
            DropForeignKey("INVENTORY.GoodsReturns", "ItemId", "INVENTORY.Items");
            DropIndex("INVENTORY.GoodsIssues", new[] { "ItemId" });
            DropIndex("INVENTORY.GoodsReturns", new[] { "ItemId" });
            DropColumn("INVENTORY.GoodsIssues", "BatchNumber");
            DropColumn("INVENTORY.GoodsIssues", "BatchDate");
            DropColumn("INVENTORY.GoodsIssues", "ItemId");
            DropColumn("INVENTORY.GoodsReturns", "BatchNumber");
            DropColumn("INVENTORY.GoodsReturns", "BatchDate");
            DropColumn("INVENTORY.GoodsReturns", "ItemId");
        }
        
        public override void Down()
        {
            AddColumn("INVENTORY.GoodsReturns", "ItemId", c => c.Long(nullable: false));
            AddColumn("INVENTORY.GoodsReturns", "BatchDate", c => c.DateTime(nullable: false));
            AddColumn("INVENTORY.GoodsReturns", "BatchNumber", c => c.String(maxLength: 100, unicode: false));
            AddColumn("INVENTORY.GoodsIssues", "ItemId", c => c.Long(nullable: false));
            AddColumn("INVENTORY.GoodsIssues", "BatchDate", c => c.DateTime(nullable: false));
            AddColumn("INVENTORY.GoodsIssues", "BatchNumber", c => c.String(maxLength: 100, unicode: false));
            CreateIndex("INVENTORY.GoodsReturns", "ItemId");
            CreateIndex("INVENTORY.GoodsIssues", "ItemId");
            AddForeignKey("INVENTORY.GoodsReturns", "ItemId", "INVENTORY.Items", "Id", cascadeDelete: true);
            AddForeignKey("INVENTORY.GoodsIssues", "ItemId", "INVENTORY.Items", "Id", cascadeDelete: true);
        }
    }
}
