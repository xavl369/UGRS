namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class GoodsIssueAndGoodsReturnWithoutCustomer : DbMigration
    {
        public override void Up()
        {
            DropForeignKey("INVENTORY.GoodsIssues", "CustomerId", "BUSINESS.Partners");
            DropForeignKey("INVENTORY.GoodsReceipts", "BatchId", "AUCTIONS.Batches");
            DropForeignKey("INVENTORY.GoodsReturns", "CustomerId", "BUSINESS.Partners");
            DropIndex("INVENTORY.GoodsIssues", new[] { "CustomerId" });
            DropIndex("INVENTORY.GoodsReceipts", new[] { "BatchId" });
            DropIndex("INVENTORY.GoodsReturns", new[] { "CustomerId" });
            AddColumn("INVENTORY.GoodsIssues", "Folio", c => c.String(maxLength: 50, unicode: false));
            AddColumn("INVENTORY.GoodsReturns", "Folio", c => c.String(maxLength: 50, unicode: false));
            DropColumn("INVENTORY.GoodsIssues", "CustomerId");
            DropColumn("INVENTORY.GoodsReceipts", "BatchId");
            DropColumn("INVENTORY.GoodsReturns", "CustomerId");
        }
        
        public override void Down()
        {
            AddColumn("INVENTORY.GoodsReturns", "CustomerId", c => c.Long(nullable: false));
            AddColumn("INVENTORY.GoodsReceipts", "BatchId", c => c.Long());
            AddColumn("INVENTORY.GoodsIssues", "CustomerId", c => c.Long(nullable: false));
            DropColumn("INVENTORY.GoodsReturns", "Folio");
            DropColumn("INVENTORY.GoodsIssues", "Folio");
            CreateIndex("INVENTORY.GoodsReturns", "CustomerId");
            CreateIndex("INVENTORY.GoodsReceipts", "BatchId");
            CreateIndex("INVENTORY.GoodsIssues", "CustomerId");
            AddForeignKey("INVENTORY.GoodsReturns", "CustomerId", "BUSINESS.Partners", "Id", cascadeDelete: true);
            AddForeignKey("INVENTORY.GoodsReceipts", "BatchId", "AUCTIONS.Batches", "Id");
            AddForeignKey("INVENTORY.GoodsIssues", "CustomerId", "BUSINESS.Partners", "Id", cascadeDelete: true);
        }
    }
}
