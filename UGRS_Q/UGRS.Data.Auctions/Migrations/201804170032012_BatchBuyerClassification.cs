namespace UGRS.Data.Auctions.Migrations
{
    using System;
    using System.Data.Entity.Migrations;
    
    public partial class BatchBuyerClassification : DbMigration
    {
        public override void Up()
        {
            AddColumn("AUCTIONS.Batches", "BuyerClassificationId", c => c.Long());
            CreateIndex("AUCTIONS.Batches", "BuyerClassificationId");
            AddForeignKey("AUCTIONS.Batches", "BuyerClassificationId", "BUSINESS.PartnerClassifications", "Id");
        }
        
        public override void Down()
        {
            DropForeignKey("AUCTIONS.Batches", "BuyerClassificationId", "BUSINESS.PartnerClassifications");
            DropIndex("AUCTIONS.Batches", new[] { "BuyerClassificationId" });
            DropColumn("AUCTIONS.Batches", "BuyerClassificationId");
        }
    }
}
