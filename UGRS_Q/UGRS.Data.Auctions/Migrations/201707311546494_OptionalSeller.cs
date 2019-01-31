namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class OptionalSeller : DbMigration
    {
        public override void Up()
        {
            DropIndex("AUCTIONS.Batches", new[] { "SellerId" });
            AddColumn("INVENTORY.GoodsIssues", "Remarks", c => c.String(maxLength: 500, unicode: false));
            AddColumn("INVENTORY.GoodsReceipts", "Remarks", c => c.String(maxLength: 500, unicode: false));
            AddColumn("INVENTORY.GoodsReturns", "Remarks", c => c.String(maxLength: 500, unicode: false));
            AlterColumn("AUCTIONS.Batches", "SellerId", c => c.Long());
            CreateIndex("AUCTIONS.Batches", "SellerId");
        }
        
        public override void Down()
        {
            DropIndex("AUCTIONS.Batches", new[] { "SellerId" });
            AlterColumn("AUCTIONS.Batches", "SellerId", c => c.Long(nullable: false));
            DropColumn("INVENTORY.GoodsReturns", "Remarks");
            DropColumn("INVENTORY.GoodsReceipts", "Remarks");
            DropColumn("INVENTORY.GoodsIssues", "Remarks");
            CreateIndex("AUCTIONS.Batches", "SellerId");
        }
    }
}
